namespace ordination_test;

using Data;
using Microsoft.EntityFrameworkCore;
using Service;
using shared.Model;

[TestClass]
public class PNTest
{

    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

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

    [TestMethod]
    public void OpretPN()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(4, service.GetPNs().Count());

        service.OpretPN(patient.PatientId, lm.LaegemiddelId, 2, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(5, service.GetPNs().Count());
    }


    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNPatientIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretPN(50, lm.LaegemiddelId, 2, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da patient id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNLægemiddelIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretPN(patient.PatientId, 50, 2, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da lægemiddel id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNPatientIdNegativ()
    {
        service.OpretPN(-1, 1, 2, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da patient id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNLægemiddelIdNegativ()
    {
        service.OpretPN(1, -1, 2, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da lægemiddel id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNAntalNegativ()
    {
        service.OpretPN(1, 1, -1, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af PN fejlet korrekt, da antal er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretPNstartDatostørreendslutDato()
    {
        service.OpretPN(1, 1, 1, DateTime.Now.AddDays(3), DateTime.Now);

        Console.WriteLine("oprettelse af PN fejlet korrekt, startDato kan ikke være større end slutDato");
    }


}