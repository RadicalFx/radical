﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;
using Radical.Validation;

namespace Radical.Tests.Validation
{
    [TestClass()]
    public class ArrayEnsureExtensionTest
    {
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void arrayEnsureExtension_containsIndex_using_an_out_of_range_index_lower()
        {
            int[] data = new int[1] { 0 };
            Ensure.That(data).ContainsIndex(-1);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void arrayEnsureExtension_containsIndex_using_an_out_of_range_index_lower_using_named_ensure()
        {
            int[] data = new int[1] { 0 };
            Ensure.That(data).Named("foo").ContainsIndex(-1);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void arrayEnsureExtension_containsIndex_using_an_out_of_range_index_upper()
        {
            int[] data = new int[1] { 0 };
            Ensure.That(data).ContainsIndex(2);
        }

        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void arrayEnsureExtension_containsIndex_using_an_out_of_range_index_upper_using_named_ensure()
        {
            int[] data = new int[1] { 0 };
            Ensure.That(data).Named("foo").ContainsIndex(2);
        }

        [TestMethod()]
        public void arrayEnsureExtension_containsIndex_using_a_valid_index()
        {
            int[] data = new int[3];
            Ensure.That(data).ContainsIndex(2);
        }

        [TestMethod]
        public void arrayEnsureExtension_containsIndex_using_using_an_out_of_range_index_and_preview_should_invoke_preview_before_throw()
        {
            var actual = false;

            try
            {
                var target = Ensure.That(new int[1] { 0 })
                    .WithPreview((v, e) => actual = true);
                target.ContainsIndex(2);
            }
            catch (ArgumentOutOfRangeException)
            {

            }

            actual.Should().Be.True();
        }
    }
}
