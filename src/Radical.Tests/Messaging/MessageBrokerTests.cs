using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ComponentModel.Messaging;
using Radical.Messaging;
using Radical.Threading;
using SharpTestsEx;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Tests.Windows.Messaging
{
    [TestClass]
    public class MessageBrokerTests
    {
        class PocoTestMessage
        {
        }

        class PocoMessageDerivedFromTestMessage : PocoTestMessage
        {
        }

        class AnotherPocoTestMessage
        {
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void
            messageBroker_POCO_unsubscribe_specific_subscriber_should_remove_only_subscriptions_for_that_subscriber()
        {
            const int expected = 1;
            var actual = 0;

            var dispatcher = new NullDispatcher();
            var target = new MessageBroker(dispatcher);

            var subscriber1 = new object();
            var subscriber2 = new object();

            target.Subscribe<PocoTestMessage>(subscriber1, (s, msg) => { actual++; });
            target.Subscribe<PocoTestMessage>(subscriber1, (s, msg) => { actual++; });
            target.Subscribe<PocoTestMessage>(subscriber1, (s, msg) => { actual++; });

            target.Subscribe<PocoTestMessage>(subscriber2, (s, msg) => { actual++; });

            target.Unsubscribe(subscriber1);

            target.Dispatch(this, new PocoTestMessage());

            actual.Should().Be.EqualTo(expected);
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void
            messageBroker_POCO_unsubscribe_specific_subscriber_and_specific_messageType_should_remove_only_subscriptions_for_that_subscriber()
        {
            const int expected = 1;
            var actual = 0;

            var dispatcher = new NullDispatcher();
            var target = new MessageBroker(dispatcher);

            var subscriber = new object();

            target.Subscribe<PocoTestMessage>(subscriber, (s, msg) => { actual++; });
            target.Subscribe<AnotherPocoTestMessage>(subscriber, (s, msg) => { actual++; });

            target.Unsubscribe<AnotherPocoTestMessage>(subscriber);

            target.Dispatch(this, new PocoTestMessage());

            actual.Should().Be.EqualTo(expected);
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void messageBroker_POCO_Dispatch_valid_message_should_not_fail()
        {
            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            broker.Dispatch(this, new PocoTestMessage());
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void messageBroker_POCO_subscribe_using_null_action_should_raise_ArgumentNullException()
        {
            Executing.This(() =>
                {
                    var dispatcher = new NullDispatcher();
                    var broker = new MessageBroker(dispatcher);

                    broker.Subscribe<PocoTestMessage>(this, (Action<object, PocoTestMessage>)null);
                })
                .Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void messageBroker_POCO_Subscribe_based_on_message_type_should_not_fail()
        {
            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            broker.Subscribe<PocoTestMessage>(this, (s, m) => { });
        }

        [TestMethod]
        public void messageBroker_POCO_subscribe_normal_should_notify()
        {
            var expected = true;
            var actual = false;

            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            broker.Subscribe<PocoTestMessage>(this, (s, msg) => actual = true);
            broker.Dispatch(this, new PocoTestMessage());

            actual.Should().Be.EqualTo(expected);
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void messageBroker_POCO_broadcast_using_more_then_one_subscriber_should_call_on_different_thread()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                Assert.Inconclusive(
                    "Test that depends on setting the ApartmentState to MTA can only be run on Windows");
                return;
            }

            TestRunner.Execute(ApartmentState.MTA, () =>
            {
                var h1 = new ManualResetEvent(false);
                var h2 = new ManualResetEvent(false);

                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                var s1ThreadId = Thread.CurrentThread.ManagedThreadId;
                var s2ThreadId = Thread.CurrentThread.ManagedThreadId;

                var dispatcher = new NullDispatcher();
                var broker = new MessageBroker(dispatcher);

                broker.Subscribe<PocoTestMessage>(this, (s, msg) =>
                {
                    s1ThreadId = Thread.CurrentThread.ManagedThreadId;
                    h1.Set();
                });

                broker.Subscribe<PocoTestMessage>(this, (s, msg) =>
                {
                    s2ThreadId = Thread.CurrentThread.ManagedThreadId;
                    h2.Set();
                });

                broker.Broadcast(this, new PocoTestMessage());

                WaitHandle.WaitAll(new[] {h1, h2});

                currentThreadId.Should().Not.Be.EqualTo(s1ThreadId);
                currentThreadId.Should().Not.Be.EqualTo(s2ThreadId);
            });
        }

        class TestRunner
        {
            readonly Action test;
            readonly ApartmentState state;
            Exception ex;

            private TestRunner(ApartmentState state, Action test)
            {
                this.state = state;
                this.test = test;
            }

            public static void Execute(ApartmentState state, Action test)
            {
                var runner = new TestRunner(state, test);
                runner.Execute();
            }

            private void Execute()
            {
                var worker = new Thread(() =>
                {
                    try
                    {
                        test();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        ex = e;
                    }
                });

                worker.SetApartmentState(state);
                worker.Start();
                worker.Join();

                if (ex != null)
                {
                    throw ex;
                }
            }
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void
            MessageBroker_POCO_subscriber_using_a_base_class_should_be_dispatched_using_a_derived_class_message()
        {
            var actual = false;

            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            broker.Subscribe<object>(this, (s, msg) => actual = true);
            broker.Dispatch(this, new PocoTestMessage());

            actual.Should().Be.True();
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void
            MessageBroker_POCO_subscriber_using_a_base_class_should_be_dispatched_using_a_derived_class_message_even_using_different_messages()
        {
            var actual = 0;

            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            broker.Subscribe<object>(this, (s, msg) => actual++);
            broker.Dispatch(this, new PocoTestMessage());
            broker.Dispatch(this, new AnotherPocoTestMessage());

            actual.Should().Be.EqualTo(2);
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void
            MessageBroker_POCO_subscriber_using_a_base_class_should_be_dispatched_only_to_the_expected_inheritance_chain()
        {
            var actual = 0;

            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            broker.Subscribe<PocoTestMessage>(this, (s, m) => actual++);
            broker.Dispatch(this, new PocoMessageDerivedFromTestMessage());
            broker.Dispatch(this, new AnotherPocoTestMessage());

            actual.Should().Be.EqualTo(1);
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void MessageBroker_dispatched_should_respect_the_given_priority()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public void MessageBroker_broadcast_from_multiple_thread_should_not_fail()
        {
            Exception failure = null;
            var wh = new ManualResetEvent(false);
            var run = true;

            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            // do some metrics to find a good number of subscription to let the pub do a long running
            const int metricsCount = 100;
            for (int i = 0; i < metricsCount; i++)
            {
                broker.Subscribe<PocoTestMessage>(this, (sender, msg) => { });
            }

            var sw = Stopwatch.StartNew();
            broker.Broadcast(this, new PocoTestMessage());
            sw.Stop();

            const int longRunningTiming = 100;
            var elapsedMilliseconds = sw.ElapsedMilliseconds == 0
                ? longRunningTiming
                : sw.ElapsedMilliseconds;

            var subCountForLongRunning = (longRunningTiming / elapsedMilliseconds) * metricsCount;
            Trace.WriteLine(string.Format("Need {0} subscriptions to run for at least {1} ms. {2} took {3} ms.",
                subCountForLongRunning, longRunningTiming, metricsCount, sw.ElapsedMilliseconds));

            for (int i = 0; i < subCountForLongRunning - metricsCount; i++)
            {
                broker.Subscribe<PocoTestMessage>(this, (sender, msg) => { });
            }

            // this will take approximately 100 ms to do 1 broadcast
            var broadcastThread1 = new Thread(payload =>
            {
                while (run)
                {
                    try
                    {
                        broker.Broadcast(this, new PocoTestMessage());
                    }
                    catch (Exception e)
                    {
                        lock (this)
                        {
                            failure = e;
                            wh.Set();
                        }

                        break;
                    }
                }
            });
            broadcastThread1.IsBackground = true;
            broadcastThread1.Start();

            // this should istantly throw an error because the broadcasting is enumerating the subscriptions and should take 100 ms to enumerate them all
            var subscriberThread1 = new Thread(payload =>
            {
                while (run)
                {
                    try
                    {
                        broker.Subscribe<PocoTestMessage>(this, (sender, msg) =>
                        {
                            Thread.Sleep(10);
                        });
                    }
                    catch (Exception e)
                    {
                        lock (this)
                        {
                            failure = e;
                            wh.Set();

                            break;
                        }
                    }
                }
            });
            subscriberThread1.IsBackground = true;
            subscriberThread1.Start();

            var timeout = 1;
            var signaled = wh.WaitOne(TimeSpan.FromSeconds(timeout));
            if (!signaled)
            {
                Trace.WriteLine(string.Format("Run without any issue for {0} seconds.", timeout));
            }

            run = false;

            subscriberThread1.Join();

            Assert.IsNull(failure, failure != null ? failure.ToString() : "--");
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        public async Task messageBroker_POCO_should_respect_should_handle_predicate()
        {
            bool actual = false;

            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            broker.Subscribe<PocoTestMessage>(this, (s, msg) => Task.FromResult(false), (s, msg) =>
            {
                actual = true;
                return Task.CompletedTask;
            });

            await broker.BroadcastAsync(this, new PocoTestMessage());

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [TestCategory("MessageBroker")]
        //BUG - Fix for: https://github.com/RadicalFx/Radical/issues/241
        public void messageBroker_should_allow_POCO_subscriptions_and_not_IMessage_ones()
        {
            var dispatcher = new NullDispatcher();
            var broker = new MessageBroker(dispatcher);

            broker.Subscribe<PocoTestMessage>(this, (s, msg) => Task.FromResult(false),
                (s, msg) =>
                {
                    return Task.CompletedTask;
                });
        }
    }
}
