namespace ordination_test;

using Service;
using shared.Model;
using Microsoft.EntityFrameworkCore;
using Data;

[TestClass]
public class DagligFastTest
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

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastPatientIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(10, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da patient id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastLægemiddelIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(patient.PatientId, 8, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da lægemiddel id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastPatientIdNegativ()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(-1, lm.LaegemiddelId, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da patient id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFastLægemiddelIdNegativ()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(patient.PatientId, -1, 2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, da lægemiddel id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligFaststartDatostørreendslutDato()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        service.OpretDagligFast(patient.PatientId, -1, 2, 2, 1, 0, DateTime.Now.AddDays(3), DateTime.Now);
        Console.WriteLine("oprettelse af daglig fast fejlet korrekt, startDato kan ikke være større end slutDato");
    }
}