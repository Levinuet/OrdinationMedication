namespace ordination_test;
using shared.Model;

[TestClass]
public class DagligSkævTest
{
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

        Assert.AreEqual(3, result);


    }

}
