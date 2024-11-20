namespace ordination_test;

using shared.Model;

[TestClass]
public class PNTest
{

    [TestMethod]
    public void GivDosisTest()
    {
        // Arrange: Opret en PN-ordination med en gyldighedsperiode
        DateTime startDen = new DateTime(2024, 11, 20);
        DateTime slutDen = new DateTime(2024, 11, 27);
        double antalEnheder = 2.0;
        Laegemiddel laegemiddel = new Laegemiddel(); // Eventuel mock eller stub
        PN pn = new PN(startDen, slutDen, antalEnheder, laegemiddel);

        // Testdatoer
        Dato gyldigDato = new Dato { dato = new DateTime(2024, 11, 22) }; // Gyldig dato
        Dato ugyldigDato = new Dato { dato = new DateTime(2024, 11, 28) }; // Ugyldig dato

        // Act & Assert: Test gyldig dato
        bool result1 = pn.givDosis(gyldigDato);
        Assert.IsTrue(result1, "Metoden bør returnere true for en dato inden for gyldighedsperioden.");
        Assert.AreEqual(1, pn.getAntalGangeGivet(), "Den gyldige dato burde være blevet tilføjet til listen.");

        // Act & Assert: Test ugyldig dato
        bool result2 = pn.givDosis(ugyldigDato);
        Assert.IsFalse(result2, "Metoden bør returnere false for en dato uden for gyldighedsperioden.");
        Assert.AreEqual(1, pn.getAntalGangeGivet(), "Den ugyldige dato burde ikke være blevet tilføjet til listen.");
    }

    [TestMethod]
    public void DoegnDosisTest()
    {
        // Arrange: Opret en PN-ordination med en gyldighedsperiode og antal enheder
        DateTime startDen = new DateTime(2024, 11, 20);
        DateTime slutDen = new DateTime(2024, 11, 27);
        double antalEnheder = 2.0;
        Laegemiddel laegemiddel = new Laegemiddel(); // Eventuel mock eller stub
        PN pn = new PN(startDen, slutDen, antalEnheder, laegemiddel);

        // Giv dosis på forskellige datoer
        pn.givDosis(new Dato { dato = new DateTime(2024, 11, 20) }); // 1. dosis
        pn.givDosis(new Dato { dato = new DateTime(2024, 11, 22) }); // 2. dosis
        pn.givDosis(new Dato { dato = new DateTime(2024, 11, 24) }); // 3. dosis
        pn.givDosis(new Dato { dato = new DateTime(2024, 11, 27) }); // 4. dosis

        // Act: Beregn den gennemsnitlige dosis per dag
        double resultat = pn.doegnDosis();

        // Beregn den forventede værdi
        int antalDage = (slutDen - startDen).Days + 1; // 8 dage i perioden
        double forventetDosis = (4 * antalEnheder) / antalDage; // 4 gange dosis á 2 enheder = 8 enheder, delt med 8 dage

        // Assert: Test om resultatet er korrekt
        Assert.AreEqual(forventetDosis, resultat, "Den gennemsnitlige dosis per dag er ikke korrekt.");
    }




}