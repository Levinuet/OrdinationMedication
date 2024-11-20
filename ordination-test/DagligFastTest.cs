namespace ordination_test;

using shared.Model;

[TestClass]
public class DagligFastTest
{

    [TestMethod]
    public void DagligDosisTest()
    {
        // Arrange: Brug seeddata fra konstruktøren
        DateTime startDen = new DateTime(2024, 11, 20);
        DateTime slutDen = new DateTime(2024, 11, 27);
        Laegemiddel laegemiddel = new Laegemiddel(); // Eventuelt udfyld med passende mock eller stub
        double morgenAntal = 1.5;
        double middagAntal = 2.0;
        double aftenAntal = 1.0;
        double natAntal = 0.5;

        // Seeddata bruges direkte i konstruktøren
        DagligFast ordination = new DagligFast(startDen, slutDen, laegemiddel, morgenAntal, middagAntal, aftenAntal, natAntal);

        // Forventet værdi baseret på seeddata
        double forventetDoegnDosis = 5;

        // Act: Beregn den daglige dosis
        double faktiskDoegnDosis = ordination.doegnDosis();

        // Assert: Sammenlign forventet og faktisk værdi
        Assert.AreEqual(forventetDoegnDosis, faktiskDoegnDosis, 0.0001, "DoegnDosis blev ikke beregnet korrekt");
    }



}