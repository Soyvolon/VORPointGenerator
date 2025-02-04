﻿using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Linq;

namespace VORPointGenerator
{

    class Program
    {

        static void Main(string[] args)
        {
            List<shipRefrence> shipRefrences = new List<shipRefrence>();
            List<gunRefrence> gunRefrences = new List<gunRefrence>();
            List<torpedoRefrence> torpedoRefrences = new List<torpedoRefrence>();
            List<missileRefrence> missileRefrences = new List<missileRefrence>();
            List<aircraftRefrence> aircraftRefrences = new List<aircraftRefrence>();
            List<shipStats> shipStats = new List<shipStats>();
            List<aircraftStats> aircraftStats = new List<aircraftStats>();
            List<missileStats> missileStats = new List<missileStats>();

            gunRefrenceList? gunRefrenceList;
            torpedoRefrenceList? torpedoRefrenceList;
            missileRefrenceList? missileRefrenceList;
            shipRefrenceList? shipRefrenceList;
            aircraftRefrenceList? aircraftRefrenceList;

            shipStatList? shipStatList;

            // String workingDirectory = Directory.GetCurrentDirectory();
            // Console.WriteLine(workingDirectory);



            // Read Weapons
            Console.WriteLine("READING WEAPON REFRENCE...");
            try
            {
                string text = File.ReadAllText(@"./jsonFiles/gunRefrences.json");
                gunRefrenceList = JsonConvert.DeserializeObject<gunRefrenceList>(text);
                gunRefrences = gunRefrenceList.gunRefrences;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find gun refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            // read torpedoes
            try
            {
                string text = File.ReadAllText(@"./jsonFiles/torpedoRefrences.json");
                torpedoRefrenceList = JsonConvert.DeserializeObject<torpedoRefrenceList>(text);
                torpedoRefrences = torpedoRefrenceList.torpedoRefrences;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find torpedo refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            // read missiles
            try
            {
                string text = File.ReadAllText(@"./jsonFiles/missileRefrences.json");
                missileRefrenceList = JsonConvert.DeserializeObject<missileRefrenceList>(text);
                missileRefrences = missileRefrenceList.missileRefrences;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find missile refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            Console.WriteLine("\nWEAPONS READ SUCESSFUL\n");

            // Read Warships
            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "READING WARSHIP REFRENCE...");

            try
            {
                string text = File.ReadAllText(@"./jsonFiles/shipRefrences.json");
                shipRefrenceList = JsonConvert.DeserializeObject<shipRefrenceList>(text);
                shipRefrences = shipRefrenceList.shipRefrences;

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find ship refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach (var i in shipRefrences)
            {
                Console.Write(i.name + ", ");
            }
            Console.WriteLine("\nWARSHIPS READ SUCCESSFUL\n");

            // Read Aircraft
            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "READING AIRCRAFT REFRENCE...");

            try
            {
                string text = File.ReadAllText(@"./jsonFiles/aircraftRefrences.json");
                aircraftRefrenceList = JsonConvert.DeserializeObject<aircraftRefrenceList>(text);
                aircraftRefrences = aircraftRefrenceList.aircraftRefrences;

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to find aircraft refrence list!");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            foreach (var i in aircraftRefrences)
            {
                Console.Write(i.name + ", ");
            }
            Console.WriteLine("\nAIRCRAFT READ SUCCESSFUL\n");



            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "GENERATING SHIP STATCARDS...");
            foreach (var i in shipRefrences)
            {
                shipStats Warship = new shipStats();
                Warship.name = i.name;
                Warship.maxSpeed = (int)Math.Round((double)i.speed / 4);
                Warship.maneuverability = (int)Math.Round((double)i.horsepower / (i.displacement * 1.5));
                // Warship.health = (int)Math.Round((double)i.displacement / 1500); //TODO: add a non-linear decay element to prevent hp values from getting too silly
                
                if(Warship.maneuverability + 2 > Warship.maxSpeed)
                {
                    double newManeuverability = (Warship.maxSpeed / 3) * 2;
                    Warship.maneuverability = (int)Math.Round(newManeuverability);
                }
                if(Warship.maneuverability < 1)
                {
                    Warship.maneuverability = 1;
                }

                // alternate health implementation
                double displacement = i.displacement;
                Warship.health = (int)Math.Round((double)i.displacement / 2000);
                while (displacement > 500)
                {
                    displacement = displacement * 0.5;
                    Warship.health++;
                }
                if (Warship.health == 1) Warship.health++;

                Warship.armor = (int)Math.Round((double)i.beltThickness / 35);
                Warship.evasion = (int)Math.Round((double)((i.length / 35) * -1) + 2  + Warship.maneuverability);

                if (i.hasSonar == true) { Warship.sonarRange = 10; }
                if (i.carrier == true) { Warship.numAircraft = (int)Math.Round(((double)i.aircraftCount / 20)); }
                else { Warship.numAircraft = (int)Math.Round((((double)i.aircraftCount + 1) / 4)); }
                if(i.carrier == true && i.steelHull == true) { Warship.numAircraft = (int)Math.Round(((double)i.aircraftCount / 15)); }
                Warship.submarine = i.submarine;
                Warship.carrier = i.carrier;
                Warship.steelHull = i.steelHull;

                Warship.shipFaction = i.shipFaction;
                Warship.hullCode = i.hullCode;

                Warship.cameo = i.cameo;
                Warship.artist = i.artist;
                Warship.artLink = i.artLink;
                // battery stats
                foreach (var j in i.batteries)
                {
                    batteryStats statBlock = new batteryStats();
                    gunRefrence batteryGun = new gunRefrence();

                    int fireControl = 0;

                    foreach (var z in gunRefrences)
                    {
                        if (z.id == j.gunRefrenceID)
                        {
                            batteryGun = z;
                        }
                    }

                    statBlock.name = batteryGun.name;
                    statBlock.turrets = j.turretCount;
                    statBlock.gunsPerTurret = j.gunsPerTurret;
                    statBlock.power = (int)Math.Round(((double)batteryGun.armorPenetration / 50));
                    statBlock.range = (int)Math.Round(((double)batteryGun.maxRange / 1750));
                    statBlock.attackAir = batteryGun.attackAir;

                    // have very high rofs increase accuracy
                    int b = batteryGun.fireRate;
                    while(b > 10) //TINKER WITH THIS A BUNCH
                    {
                        b = (b / 10);
                        fireControl++;
                    }

                    // have large guns reduce accuracy
                    double o = batteryGun.calibre;
                    while (o > 100)
                    {
                        o = (o * 0.75);
                        fireControl--;
                    }

                    if (j.localOpticalDirector) { fireControl++; }
                    if (j.localRadarDirector) { fireControl++; }
                    if (j.opticalDirector) { fireControl++; }
                    if (j.radarDirector) { fireControl++; }
                    if (j.rangefinder) { fireControl++; }
                    if (j.radarRangeFinder) { fireControl++; }
                    if (j.radarBlindFire) { fireControl++; }
                    if (j.mechanicalComputer) { fireControl++; }
                    if (j.digitalComputer) { fireControl+=2; }
                    if (j.airburstFuses) { fireControl++; }
                    if (j.radarFuses) { fireControl++; }
                    if (j.CWISTracking) { fireControl+=10; }
                    if (j.poorShellQuaility) { fireControl--; }
                    if (batteryGun.laser) { fireControl += 10; }
                    statBlock.accuracy = fireControl + ((j.gunsPerTurret - 1) * -1);

                    statBlock.applyFixes();

                    Warship.gunBatteries.Add(statBlock);

                }
                //Console.WriteLine("Number of loaded torpedoes: " + i.torpedoes.Count);
                foreach (var j in i.torpedoes)
                {
                    torpedoStats t = new torpedoStats();
                    torpedoRefrence tRef = new torpedoRefrence();

                    int torpFireControl = 0;

                    foreach (var z in torpedoRefrences)
                    {
                        if (z.id == j.torpRefrenceID)
                        {
                            tRef = z;
                            //Console.WriteLine(z.id);
                            //Console.WriteLine("found id!");
                        }
                    }

                    // increase torpfireControl if it is guided
                    if (j.torpedoDirector) { torpFireControl++; }
                    if (tRef.selfGuided) { torpFireControl = torpFireControl + 4; }
                    torpFireControl = torpFireControl + tRef.torpGuidance;

                    t.name = tRef.name;
                    t.torpPower = (int)Math.Round((double)tRef.torpWarheadSize / 50);
                    t.torpTurrets = j.turretCount;
                    t.torpsPerTurret = j.torpsPerTurret;
                    t.torpRange = (int)Math.Round(((double)tRef.torpRange / 1500));
                    t.torpAcc = torpFireControl;
                    t.torpAOE = (int)Math.Round(((double)tRef.torpSpeed / 15));
                    t.torpCharges = j.torpReloads;


                    Warship.torpedoBatteries.Add(t);
                }
                // missile stats
                foreach (var j in i.missiles)
                {
                    missileStats m = new missileStats();
                    missileRefrence mRef = new missileRefrence();

                    int mslFireCtrl = 0;

                    foreach (var z in missileRefrences)
                    {
                        if (z.id == j.missileRefrenceID)
                        {
                            mRef = z;
                            // Console.WriteLine(z.id);
                            // Console.WriteLine("found id!");
                        }
                    }


                    if (j.dataLink) mslFireCtrl+=2;
                    if (mRef.sarhGuidance) mslFireCtrl += 3;
                    if (mRef.arhGuidance) mslFireCtrl += 3;
                    if (mRef.gpsGuidance) mslFireCtrl += 2;
                    if (mRef.inertialGuidance) mslFireCtrl += 2;
                    if (mRef.infraredGuidance) mslFireCtrl += 2;
                    if (mRef.opticalGuidance) mslFireCtrl += 2;
                    if (mRef.attackAir) mslFireCtrl += 2;
                    if (mRef.cwis) mslFireCtrl += 6;
                    if (mRef.homeOnJam) mslFireCtrl += 4;
                    if (mRef.antiRadiation) mslFireCtrl += 4;
                    if (mRef.dataLinkSwarm) mslFireCtrl += 3;
                    if (mRef.antiBallistic) mslFireCtrl += 3;
                    if (mRef.sra2a) mslFireCtrl += 7;

                    //Console.WriteLine(mRef.name);

                    m.name = mRef.name;
                    m.mslPower = (int)Math.Round((double)mRef.mslWarheadSize / 50);
                    m.mslTurrets = j.turretCount;
                    m.mslsPerTurret = j.missilesPerTurret;
                    m.mslRange = (int)Math.Round(((double)mRef.mslRange / 1500));
                    m.mslAcc = mslFireCtrl - (int)Math.Round((double)mRef.mslSpeed / 500);
                    m.mslAOE = (int)Math.Round((double)mRef.mslSpeed / 100) + mslFireCtrl;
                    m.mslEvasion = (int)Math.Round(((double)mRef.mslSpeed / 75));

                    m.attackAir = mRef.attackAir;

                    if (mRef.stealth) m.mslEvasion += 5;
                    if (mRef.seaSkimming) m.mslEvasion += 3;

                    while (m.mslAOE > m.mslRange) m.mslAOE--; m.mslAcc++;

                    Warship.missileBatteries.Add(m);
                }


                //TODO: depth charges

                // Console.WriteLine(i.name + ": " + i.specialAbilities.Count);
                foreach (var j in i.specialAbilities)
                {
                    // TODO: Investigate this
                    specialAbility s = new specialAbility();
                    s.name = j.name;
                    s.description = j.description;
                    Warship.specialAbilities.Add(s);
                }

                Warship.abilityWeight = i.abilityWeight;

                Warship.calculatePointValue();
                

                //Console.WriteLine(Warship.printStats());

                Warship.generateStatCard();
                
                shipStats.Add(Warship);
            }

            //generate aircraft
            Console.WriteLine("\n\t-\t-\t-\n\n" +
                "GENERATING AIRCRAFT STATCARDS...\n");

            foreach (var i in aircraftRefrences)
            {
                aircraftStats Aircraft = new aircraftStats();
                Aircraft.name = i.name;
                Aircraft.type = i.type;
                Aircraft.countryOfOrigin = i.countryOfOrigin;
                Aircraft.planecount = i.numPlanes;
                //.move = (int)Math.Round(((double)i.speed / 7));  // old

                double speed = i.speed;
                Aircraft.move = (int)Math.Round((speed / 18));

                if (Aircraft.move > 10)
                {
                    Aircraft.move = 0;
                    while (speed > 10)
                    {
                        speed = speed * 0.8;
                        Aircraft.move++;
                    }
                }

                double TWRatio = ((double)i.thrust / (double)i.weight);

                //TODO: final values feel wonky, adjust bias number as needed once more aircraft have been added
                
                Aircraft.energyGain = (int)Math.Round(Math.Pow(((double)TWRatio * i.rateOfClimb * i.rateOfClimb * 0.0196210657), 0.5));
                if (Aircraft.energyGain > 5)
                {
                    double gainEnergy = TWRatio * i.rateOfClimb * i.rateOfClimb;
                    Aircraft.energyGain = (int)(TWRatio * i.rateOfClimb * i.rateOfClimb * 0.000002);
                    while (gainEnergy > 10)
                    {
                        gainEnergy = gainEnergy * 0.2;
                        Aircraft.energyGain++;
                    }
                }

                Aircraft.maxEnergy = (int)Math.Round((((double)i.speed * i.serviceCieling) / 250000));
                if (Aircraft.maxEnergy > 20)
                {
                    // Console.WriteLine(Aircraft.name + ": " + Aircraft.maxEnergy + " Recalcuating Energy");


                    Aircraft.maxEnergy = (int)Math.Round((((double)i.speed * i.serviceCieling) / 1500000));
                    double energyMax = i.speed * i.serviceCieling;
                    while (energyMax > 20)
                    {
                        energyMax = energyMax * 0.5;
                        Aircraft.maxEnergy++;
                    }
                    // Console.WriteLine("New energy: " + Aircraft.maxEnergy);
                }

                Aircraft.cameo = i.cameo;
                Aircraft.artist = i.artist;
                Aircraft.artLink = i.artLink;

                Aircraft.abyssalName = i.abyssalName;
                Aircraft.abyssalCameo = i.abyssalCameo;
                Aircraft.abyssalArtist = i.abyssalArtist;
                Aircraft.abyssalArtLink = i.abyssalArtLink;

                //batteries
                foreach (var j in i.gunRefrences)
                {
                    batteryStats statBlock = new batteryStats();
                    gunRefrence batteryGun = new gunRefrence();

                    int fireControl = 0;

                    foreach (var z in gunRefrences)
                    {
                        if (z.id == j.gunRefrenceID)
                        {
                            batteryGun = z;
                        }
                    }

                    statBlock.name = batteryGun.name;
                    statBlock.turrets = j.turretCount;
                    statBlock.gunsPerTurret = j.gunsPerTurret;
                    statBlock.power = (int)Math.Round(((double)batteryGun.armorPenetration / 50));
                    statBlock.range = (int)Math.Round(((double)batteryGun.maxRange / 1750));

                    // have very high rofs increase accuracy
                    int b = batteryGun.fireRate;
                    while (b > 10) //TINKER WITH THIS A BUNCH
                    {
                        b = (b / 10);
                        fireControl++;
                    }

                    if (j.localOpticalDirector) { fireControl++; }
                    if (j.localRadarDirector) { fireControl++; }
                    if (j.opticalDirector) { fireControl++; }
                    if (j.radarDirector) { fireControl++; }
                    if (j.rangefinder) { fireControl++; }
                    if (j.radarRangeFinder) { fireControl++; }
                    if (j.radarBlindFire) { fireControl++; }
                    if (j.mechanicalComputer) { fireControl++; }
                    if (j.digitalComputer) { fireControl++; }
                    if (j.airburstFuses) { fireControl++; }
                    if (j.radarFuses) { fireControl++; }
                    if (j.poorShellQuaility) { fireControl--; }

                    // reduce number of guns, but increase accuracy
                    while (statBlock.gunsPerTurret > 1)
                    {
                        statBlock.gunsPerTurret --;
                        fireControl++;
                    }


                    statBlock.accuracy = fireControl + ((statBlock.gunsPerTurret - 1) / 2 * -1);

                    statBlock.applyFixes();

                    Aircraft.gunStats.Add(statBlock);

                }

                // Add rockets
                foreach (var j in i.rocketRefrences)
                { 
                    rocketStats rocket = new rocketStats();
                    rocket.name = j.rocketName;
                    rocket.rocketAtk = j.rocketVolleySize;
                    rocket.rocketVolleys = (int)Math.Round((double)j.rocketNumber / j.rocketVolleySize);
                    rocket.rocketRange = (int)Math.Round((double)j.rocketMaxRange / 500);
                    rocket.rocketPower = (int)Math.Round((double)j.rocketWarheadSize / 50);

                    int fireControl = 1;

                    if (j.railDropped) { fireControl-=2; }

                    int totalRockets = j.rocketNumber * i.numPlanes;

                    while (totalRockets > 40)
                    {
                        rocket.rocketVolleys = rocket.rocketVolleys / 2;
                        fireControl += 2;

                        // Console.WriteLine("Looptest: " + Aircraft.name + " " + rocket.rocketVolleys);

                        totalRockets = rocket.rocketAtk * rocket.rocketVolleys * i.numPlanes;
                    }

                    rocket.rocketAcc = fireControl - j.rocketVolleySize;


                    Aircraft.rocketStats.Add(rocket);
                }
                
                foreach (var j in i.bombRefrences)
                {
                    bombStats bomb = new bombStats();
                    bomb.name = j.name;
                    bomb.atk = j.bombVolleySize;
                    bomb.volleys = (int)Math.Round((double)j.number / j.bombVolleySize);
                    bomb.power = (int)Math.Round((double)j.bombWarheadSize / 50);

                    int bombGuidance = 0;
                    if(j.laserGuidance == true) { bombGuidance ++; }

                    bomb.diveBomb = j.diveBomb;

                    bomb.accuracy = bombGuidance;

                    Aircraft.bombStats.Add(bomb);
                }
                // add torpedoes
                foreach (var j in i.torpedoBatRefrences)
                {
                    torpedoStats t = new torpedoStats();
                    torpedoRefrence tRef = new torpedoRefrence();

                    //Console.WriteLine(i.name); 

                    int torpFireControl = 0;

                    foreach (var z in torpedoRefrences)
                    {
                        if (z.id == j.torpRefrenceID)
                        {
                            tRef = z;
                        }
                    }

                    // increase torpfireControl if it is guided
                    if (j.torpedoDirector) { torpFireControl++; }
                    if (tRef.selfGuided) { torpFireControl = torpFireControl + 4; }
                    torpFireControl = torpFireControl + tRef.torpGuidance;

                    t.name = tRef.name;
                    t.torpPower = (int)Math.Round((double)tRef.torpWarheadSize / 50);
                    t.torpTurrets = j.turretCount;
                    t.torpsPerTurret = j.torpsPerTurret;
                    t.torpRange = (int)Math.Round(((double)tRef.torpRange / 1500));
                    t.torpAcc = torpFireControl;
                    t.torpAOE = (int)Math.Round(((double)tRef.torpSpeed / 15));
                    t.torpCharges = j.torpReloads;

                    Aircraft.torpedoStats.Add(t);
                }

                // Add missiles
                foreach (var j in i.missiles)
                {
                    missileStats m = new missileStats();
                    missileRefrence mRef = new missileRefrence();

                    int mslFireCtrl = 0;

                    foreach (var z in missileRefrences)
                    {
                        if (z.id == j.missileRefrenceID)
                        {
                            mRef = z;
                        }
                    }

                    if (j.dataLink) mslFireCtrl += 2;
                    if (mRef.sarhGuidance) mslFireCtrl += 3;
                    if (mRef.arhGuidance) mslFireCtrl += 3;
                    if (mRef.gpsGuidance) mslFireCtrl += 3;
                    if (mRef.inertialGuidance) mslFireCtrl += 3;
                    if (mRef.infraredGuidance) mslFireCtrl += 3;
                    if (mRef.opticalGuidance) mslFireCtrl += 3;
                    if (mRef.attackAir) mslFireCtrl += 3;
                    if (mRef.cwis) mslFireCtrl += 6;
                    if (mRef.homeOnJam) mslFireCtrl += 4;
                    if (mRef.antiRadiation) mslFireCtrl += 4;
                    if (mRef.dataLinkSwarm) mslFireCtrl += 4;
                    if (mRef.antiBallistic) mslFireCtrl += 3;
                    if (mRef.sra2a) mslFireCtrl += 7;

                    //Console.WriteLine(mRef.name);

                    m.name = mRef.name;
                    m.mslPower = (int)Math.Round((double)mRef.mslWarheadSize / 50);
                    m.mslTurrets = j.turretCount;
                    m.mslsPerTurret = j.missilesPerTurret;
                    m.mslRange = (int)Math.Round(((double)mRef.mslRange / 1500));
                    m.mslAcc = mslFireCtrl - (int)Math.Round((double)mRef.mslSpeed / 500);
                    m.mslAOE = (int)Math.Round((double)mRef.mslSpeed / 100) + mslFireCtrl;
                    m.mslEvasion = (int)Math.Round(((double)mRef.mslSpeed / 75));

                    m.attackAir = mRef.attackAir;

                    if (mRef.stealth) m.mslEvasion += 8;
                    if (mRef.seaSkimming) m.mslEvasion += 3;

                    Aircraft.missileStats.Add(m);
                }

                // Console.WriteLine(i.name + ": " + i.specialAbilities.Count);
                foreach (var j in i.specialAbilities)
                {
                    // TODO: Investigate this
                    specialAbility s = new specialAbility();
                    s.name = j.name;
                    s.description = j.description;
                    Aircraft.specialAbilities.Add(s);
                }

                Aircraft.abilityWeight = i.abilityWeight;
                // Calculate cost
                Aircraft.calculatePointValue();

                // Generate a stat card for the normal aircraft
                Aircraft.generateStatCard(Aircraft.name, Aircraft.countryOfOrigin, Aircraft.cameo, Aircraft.artist, Aircraft.artLink);

                // Generate abyssal version of this statcard
                if (Aircraft.abyssalName  != string.Empty)
                {
                    Aircraft.generateStatCard(Aircraft.abyssalName, "ABYSSAL", Aircraft.abyssalCameo, Aircraft.abyssalArtist, Aircraft.abyssalArtLink);
                }

                //Console.WriteLine(Aircraft.printStats());

                aircraftStats.Add(Aircraft);
            }
        }

        //TODO: Generate a lua script that spawns a notecard statblock for each ship and aircraft
    }
}