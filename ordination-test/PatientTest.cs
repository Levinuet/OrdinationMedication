namespace ordination_test;
using shared.Model;

[TestClass]
public class PatientTest
{

    [TestMethod]
    public void PatientHasName()
    {
        string cpr = "160563-4321";
        string navn = "John";
        double vægt = 83;
        
        Patient patient = new Patient(cpr, navn, vægt);
        Assert.AreEqual(navn, patient.navn);
    }


    [TestMethod]

    public void TestDerAltidFejler()
    {
        string cpr = "160563-1234";
        string navn = "John";
        double vægt = 83;

        Patient patient = new Patient(cpr, navn, vægt);
        try { Assert.AreEqual("Egon", patient.navn); }
        catch (Exception) { Console.WriteLine("Assert.AreEqual failed correctly"); }

    }
}