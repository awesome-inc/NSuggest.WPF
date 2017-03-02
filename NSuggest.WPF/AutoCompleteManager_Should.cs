using System;
using System.Threading;
using System.Windows.Controls;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NSuggest.WPF
{
    [TestFixtureFor(typeof(AutoCompleteManager))]
    // ReSharper disable InconsistentNaming
    internal class AutoCompleteManager_Should
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void a()
        {
            0.Invoking(x => new AutoCompleteManager(null)).ShouldThrow<ArgumentNullException>();

            Assert.Inconclusive("TODO");

            var textBox = new TextBox();
            var sut = new AutoCompleteManager(textBox);
        }
    }
}