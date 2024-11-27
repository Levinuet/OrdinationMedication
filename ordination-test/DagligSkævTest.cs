namespace ordination_test;

using Data;
using Microsoft.EntityFrameworkCore;
using Service;
using shared.Model;

[TestClass]
public class DagligSkævTest
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
    public void OpretDagligSkaev()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligSkæve().Count());

        Dosis[] doser = new Dosis[] { new Dosis(new DateTime(2024, 1, 1, 12, 0, 0), 2) };
        service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligSkæve().Count());
    }

    [TestMethod]
    public void doegnDosis()
    {
        //Her skal oprettes test efter en test metode, som skal teste doegnDosis metoden i DagligSkæv klassen ud fra de 3 A'er.
        //Arrange

        Laegemiddel laegemiddel = new Laegemiddel("Paracetamol", 1, 1.5, 2, "Ml");

        DateTime startDen = new DateTime(2024, 1, 1);
        DateTime slutDen = new DateTime(2024, 1, 3);

        Dosis[] doser = new Dosis[3];

        doser[0] = new Dosis(new DateTime(2024, 1, 1), 2);
        doser[1] = new Dosis(new DateTime(2024, 1, 2), 3);
        doser[2] = new Dosis(new DateTime(2024, 1, 3), 4);

        DagligSkæv dagligSkæv = new DagligSkæv(startDen, slutDen, laegemiddel, doser);

        //Act

        double result = dagligSkæv.doegnDosis();

        //Assert

        Assert.AreEqual(9, result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævPatientIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis[] doser = new Dosis[] { new Dosis(new DateTime(2024, 1, 1, 12, 0, 0), 2) };
        service.OpretDagligSkaev(10, lm.LaegemiddelId, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da patient id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævLægemiddelIdFindesIkke()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        Dosis[] doser = new Dosis[] { new Dosis(new DateTime(2024, 1, 1, 12, 0, 0), 2) };
        service.OpretDagligSkaev(patient.PatientId, 10, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da lægemiddel id ikke findes");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævPatientIdNegativ()
    {
        Dosis[] doser = new Dosis[] { new Dosis(new DateTime(2024, 1, 1, 12, 0, 0), 2) };
        service.OpretDagligSkaev(-1, 1, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da patient id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævLægemiddelIdNegativ()
    {
        Dosis[] doser = new Dosis[] { new Dosis(new DateTime(2024, 1, 1, 12, 0, 0), 2) };
        service.OpretDagligSkaev(1, -1, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da lægemiddel id er negativ");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkævDoserTom()
    {
        Dosis[] doser = new Dosis[] { };
        service.OpretDagligSkaev(1, 1, doser, DateTime.Now, DateTime.Now.AddDays(3));
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, da doser er tom");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void OpretDagligSkaevstartDatostørreendslutDato()
    {
        Dosis[] doser = new Dosis[] { new Dosis(new DateTime(2024, 1, 1, 12, 0, 0), 2) };
        service.OpretDagligSkaev(1, -1, doser, DateTime.Now.AddDays(3), DateTime.Now);
        Console.WriteLine("oprettelse af daglig skæv fejlet korrekt, startDato kan ikke være større end slutDato");
    }
}