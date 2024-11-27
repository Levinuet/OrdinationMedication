using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shared.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ordination_test
{
    [TestClass]
    public class OrdinationTest
    {

        [TestMethod]

        public void antalDagetest()
        {
            // Arrange
            var start = new DateTime(2024, 11, 1);
            var end = new DateTime(2024, 11, 20);
            var ordination = new TestOrdination(null!, start, end);

            // Act
            int result = ordination.antalDage();

            // Assert
            Assert.AreEqual(20, result, "Antal dage skal være 1, når start- og slutdato er den samme.");
        }

    }

    public class TestOrdination : Ordination
    {
        public TestOrdination(Laegemiddel laegemiddel, DateTime start, DateTime end)
            : base(laegemiddel, start, end) { }

        public override double samletDosis() => throw new NotImplementedException();
        public override double doegnDosis() => throw new NotImplementedException();
        public override string getType() => throw new NotImplementedException();
    }
}
