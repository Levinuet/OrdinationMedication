using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using shared.Model;
using static shared.Util;
using Data;

namespace Service;

public class DataService
{
    private OrdinationContext db { get; }

    public DataService(OrdinationContext db) {
        this.db = db;
    }

    /// <summary>
    /// Seeder noget nyt data i databasen, hvis det er nødvendigt.
    /// </summary>
    public void SeedData() {

        // Patients
        Patient[] patients = new Patient[5];
        patients[0] = db.Patienter.FirstOrDefault()!;

        if (patients[0] == null)
        {
            patients[0] = new Patient("121256-0512", "Jane Jensen", 63.4);
            patients[1] = new Patient("070985-1153", "Finn Madsen", 83.2);
            patients[2] = new Patient("050972-1233", "Hans Jørgensen", 89.4);
            patients[3] = new Patient("011064-1522", "Ulla Nielsen", 59.9);
            patients[4] = new Patient("123456-1234", "Ib Hansen", 87.7);

            db.Patienter.Add(patients[0]);
            db.Patienter.Add(patients[1]);
            db.Patienter.Add(patients[2]);
            db.Patienter.Add(patients[3]);
            db.Patienter.Add(patients[4]);
            db.SaveChanges();
        }

        Laegemiddel[] laegemiddler = new Laegemiddel[5];
        laegemiddler[0] = db.Laegemiddler.FirstOrDefault()!;
        if (laegemiddler[0] == null)
        {
            laegemiddler[0] = new Laegemiddel("Acetylsalicylsyre", 0.1, 0.15, 0.16, "Styk");
            laegemiddler[1] = new Laegemiddel("Paracetamol", 1, 1.5, 2, "Ml");
            laegemiddler[2] = new Laegemiddel("Fucidin", 0.025, 0.025, 0.025, "Styk");
            laegemiddler[3] = new Laegemiddel("Methotrexat", 0.01, 0.015, 0.02, "Styk");
            laegemiddler[4] = new Laegemiddel("Prednisolon", 0.1, 0.15, 0.2, "Styk");

            db.Laegemiddler.Add(laegemiddler[0]);
            db.Laegemiddler.Add(laegemiddler[1]);
            db.Laegemiddler.Add(laegemiddler[2]);
            db.Laegemiddler.Add(laegemiddler[3]);
            db.Laegemiddler.Add(laegemiddler[4]);

            db.SaveChanges();
        }

        Ordination[] ordinationer = new Ordination[6];
        ordinationer[0] = db.Ordinationer.FirstOrDefault()!;
        if (ordinationer[0] == null) {
            Laegemiddel[] lm = db.Laegemiddler.ToArray();
            Patient[] p = db.Patienter.ToArray();

            ordinationer[0] = new PN(new DateTime(2024, 11, 29), new DateTime(2024, 11, 30), 4, lm[1]);    
            ordinationer[1] = new PN(new DateTime(2024, 11, 29), new DateTime(2024, 11, 30), 3, lm[0]);    
            ordinationer[2] = new PN(new DateTime(2024, 11, 29), new DateTime(2024, 11, 30), 5, lm[2]);    
            ordinationer[3] = new PN(new DateTime(2024, 11, 29), new DateTime(2024, 11, 30), 4, lm[1]);
            ordinationer[4] = new DagligFast(new DateTime(2024, 11, 29), new DateTime(2024, 11, 30), lm[1], 2, 0, 1, 0);
            ordinationer[5] = new DagligSkæv(new DateTime(2024, 11, 29), new DateTime(2024, 11, 30), lm[2]);
            
            ((DagligSkæv) ordinationer[5]).doser = new Dosis[] { 
                new Dosis(CreateTimeOnly(12, 0, 0), 0.5),
                new Dosis(CreateTimeOnly(12, 40, 0), 1),
                new Dosis(CreateTimeOnly(16, 0, 0), 2.5),
                new Dosis(CreateTimeOnly(18, 45, 0), 3)        
            }.ToList();
            

            db.Ordinationer.Add(ordinationer[0]);
            db.Ordinationer.Add(ordinationer[1]);
            db.Ordinationer.Add(ordinationer[2]);
            db.Ordinationer.Add(ordinationer[3]);
            db.Ordinationer.Add(ordinationer[4]);
            db.Ordinationer.Add(ordinationer[5]);

            db.SaveChanges();

            p[0].ordinationer.Add(ordinationer[0]);
            p[0].ordinationer.Add(ordinationer[1]);
            p[2].ordinationer.Add(ordinationer[2]);
            p[3].ordinationer.Add(ordinationer[3]);
            p[1].ordinationer.Add(ordinationer[4]);
            p[1].ordinationer.Add(ordinationer[5]);

            db.SaveChanges();
        }
    }

    
    public List<PN> GetPNs() {
        return db.PNs.Include(o => o.laegemiddel).Include(o => o.dates).ToList();
    }

    public List<DagligFast> GetDagligFaste() {
        return db.DagligFaste
            .Include(o => o.laegemiddel)
            .Include(o => o.MorgenDosis)
            .Include(o => o.MiddagDosis)
            .Include(o => o.AftenDosis)            
            .Include(o => o.NatDosis)            
            .ToList();
    }

    public List<DagligSkæv> GetDagligSkæve() {
        return db.DagligSkæve
            .Include(o => o.laegemiddel)
            .Include(o => o.doser)
            .ToList();
    }

    public List<Patient> GetPatienter() {
        return db.Patienter.Include(p => p.ordinationer).ToList();
    }

    public List<Laegemiddel> GetLaegemidler() 
    {
        return db.Laegemiddler.ToList();
    }

    public Laegemiddel GetLaegemiddel(int id)
    {
        return db.Laegemiddler.First(p => p.LaegemiddelId == id);
    }

    public PN OpretPN(int patientId, int laegemiddelId, double antal, DateTime startDato, DateTime slutDato)
    {

        if (patientId < 0 || laegemiddelId < 0 || antal < 0 || startDato > slutDato)
        {
            throw new InvalidOperationException("Id kan ikke være et negativt integer");
            return null;
        }
        else
        {
            Patient p;
            Laegemiddel lm;

            try
            {
                lm = db.Laegemiddler.First(lm => lm.LaegemiddelId == laegemiddelId);
            }

            catch
            {
                throw new InvalidOperationException("lægemiddel findes ikke");
            }

            try
            {
                p = db.Patienter.First(p => p.PatientId == patientId);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("patient findes ikke");
            }

            PN addPN = new PN(startDato, slutDato, antal, GetLaegemiddel(laegemiddelId));
            p.ordinationer.Add(addPN);
            db.SaveChanges();

            return addPN!;
        }
    }

    public DagligFast OpretDagligFast(int patientId, int laegemiddelId,
        double antalMorgen, double antalMiddag, double antalAften, double antalNat,
        DateTime startDato, DateTime slutDato)
    {
        if (patientId < 0 || laegemiddelId < 0 || startDato > slutDato)
        {
            throw new InvalidOperationException("Id kan ikke være et negativt integer");
            return null;
        }
        else
        {
            Patient p;
            Laegemiddel lm;

            try
            {
                lm = db.Laegemiddler.First(lm => lm.LaegemiddelId == laegemiddelId);
            }

            catch
            {
                throw new InvalidOperationException("lægemiddel findes ikke");
            }

            try
            {
                p = db.Patienter.First(p => p.PatientId == patientId);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("patient findes ikke");
            }

            DagligFast addDagligFast = new DagligFast(startDato, slutDato, lm, antalMorgen, antalMiddag, antalAften, antalNat);
            p.ordinationer.Add(addDagligFast);
            db.SaveChanges();
            return addDagligFast!;
        }

    }

    public DagligSkæv OpretDagligSkaev(int patientId, int laegemiddelId, Dosis[] doser, DateTime startDato, DateTime slutDato)
    {
        if (patientId < 0 || laegemiddelId < 0 || doser.Length == 0 || startDato > slutDato)
        {
            throw new InvalidOperationException("Id kan ikke være et negativt integer");
            return null;
        }
        else
        {
            Patient p;
            Laegemiddel lm;

            try
            {
                lm = db.Laegemiddler.First(lm => lm.LaegemiddelId == laegemiddelId);
            }

            catch
            {
                throw new InvalidOperationException("lægemiddel findes ikke");
            }

            try
            {
                p = db.Patienter.First(p => p.PatientId == patientId);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("patient findes ikke");
            }
            DagligSkæv addDagligSkæv = new DagligSkæv(startDato, slutDato, GetLaegemiddel(laegemiddelId), doser);
            p.ordinationer.Add(addDagligSkæv);
            db.SaveChanges();
            return addDagligSkæv!;

        }
    }

    public string AnvendOrdination(int id, Dato dato)
    {
        PN pn = db.PNs.Find(id);
        bool anvendtOrdination = pn.givDosis(dato);
        if (anvendtOrdination)
        {
            db.SaveChanges();
            return "Ordination anvendt.";
        }
        else return "Ordination ikke anvendt.";

    }

    /// <summary>
    /// Den anbefalede dosis for den pågældende patient, per døgn, hvor der skal tages hensyn til
	/// patientens vægt. Enheden afhænger af lægemidlet. Patient og lægemiddel må ikke være null.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="laegemiddel"></param>
    /// <returns></returns>
	public double GetAnbefaletDosisPerDøgn(int patientId, int laegemiddelId)
    {
        Laegemiddel laegemiddel = GetLaegemiddel(laegemiddelId);
        Patient patient = db.Patienter.Find(patientId);
        if (laegemiddel != null && patient != null)
        {
            if (patient.vaegt < 25)
            {
                return laegemiddel.enhedPrKgPrDoegnLet * patient.vaegt;
            }
            else if (patient.vaegt <= 120)
            {
                return laegemiddel.enhedPrKgPrDoegnNormal * patient.vaegt;
            }
            else return laegemiddel.enhedPrKgPrDoegnTung * patient.vaegt;
        }
        return -1;
    }

    public int GetAntalOrdinationer(double vægtFra, double vægtTil, string laegemiddelNavn)
    {
        return db.Patienter
            .Where(p => p.vaegt >= vægtFra && p.vaegt <= vægtTil)
            .SelectMany(p => p.ordinationer)
            .Where(o => o.laegemiddel.navn == laegemiddelNavn)
            .Count();
    }

    public Dictionary<string, double> GetTotalMængdeOrdineretPrLægemiddel()
    {
        var result = new Dictionary<string, double>();

        var pns = db.PNs.Include(pn => pn.laegemiddel).ToList();
        foreach (var pn in pns)
        {
            if (result.ContainsKey(pn.laegemiddel.navn))
                result[pn.laegemiddel.navn] += pn.antalEnheder;
            else
                result[pn.laegemiddel.navn] = pn.antalEnheder;
        }

        var dagligFaste = db.DagligFaste.Include(df => df.laegemiddel).ToList();
        foreach (var df in dagligFaste)
        {
            double total = df.MorgenDosis.antal + df.MiddagDosis.antal + df.AftenDosis.antal + df.NatDosis.antal;
            if (result.ContainsKey(df.laegemiddel.navn))
                result[df.laegemiddel.navn] += total;
            else
                result[df.laegemiddel.navn] = total;
        }

        var dagligSkæve = db.DagligSkæve.Include(ds => ds.laegemiddel).ToList();
        foreach (var ds in dagligSkæve)
        {
            double total = ds.doser.Sum(d => d.antal);
            if (result.ContainsKey(ds.laegemiddel.navn))
                result[ds.laegemiddel.navn] += total;
            else
                result[ds.laegemiddel.navn] = total;
        }

        return result;
    }
}