namespace shared.Model;

public class DagligSkæv : Ordination {
    public List<Dosis> doser { get; set; } = new List<Dosis>();

    public DagligSkæv(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
	}

    public DagligSkæv(DateTime startDen, DateTime slutDen, Laegemiddel laegemiddel, Dosis[] doser) : base(laegemiddel, startDen, slutDen) {
        this.doser = doser.ToList();
    }    

    public DagligSkæv() : base(null!, new DateTime(), new DateTime()) {
    }

	public void opretDosis(DateTime tid, double antal) {
        doser.Add(new Dosis(tid, antal));
    }

	public override double samletDosis() {
		return base.antalDage() * doegnDosis();
	}

    public override double doegnDosis()
    {
        int antalDage = base.antalDage();

        if (antalDage <= 0)
        {
            throw new InvalidOperationException("Antal dage must be greater than zero.");
        }

        double samletDosis = 0;

        if (doser != null && doser.Count > 0)
        {
            foreach (Dosis dosis in doser)
            {
                samletDosis += dosis.antal;
            }
        }
        else
        {
            throw new InvalidOperationException("Doser list must not be empty.");
        }

        return samletDosis / antalDage;
    }

    public override String getType() {
		return "DagligSkæv";
	}
}
