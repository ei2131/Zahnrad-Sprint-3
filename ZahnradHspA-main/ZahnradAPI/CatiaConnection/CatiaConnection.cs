﻿using System;

using HybridShapeTypeLib;

using INFITF;

using MECMOD;

using PARTITF;

using ProductStructureTypeLib;

using KnowledgewareTypeLib;

using System.Windows;







namespace Zahnrad

{











    class CatiaConnection

    {

        INFITF.Application hsp_catiaApp;

        MECMOD.PartDocument hsp_catiaPart;

        MECMOD.Sketch hsp_catiaProfil;



        public bool CATIALaeuft()

        {

            try

            {

                object catiaObject = System.Runtime.InteropServices.Marshal.GetActiveObject(

                    "CATIA.Application");

                hsp_catiaApp = (INFITF.Application)catiaObject;

                return true;

            }

            catch (Exception)

            {

                return false;

            }

        }



        public Boolean ErzeugePart()

        {

            INFITF.Documents catDocuments1 = hsp_catiaApp.Documents;

            hsp_catiaPart = catDocuments1.Add("Part") as MECMOD.PartDocument;

            return true;

        }



        public void ErstelleLeereSkizze()

        {

            // geometrisches Set auswaehlen und umbenennen

            HybridBodies catHybridBodies1 = hsp_catiaPart.Part.HybridBodies;

            HybridBody catHybridBody1;

            try

            {

                catHybridBody1 = catHybridBodies1.Item("Geometrisches Set.1");

            }

            catch (Exception)

            {

                MessageBox.Show("Kein geometrisches Set gefunden! " + Environment.NewLine +

                    "Ein PART manuell erzeugen und ein darauf achten, dass Geometisches Set' aktiviert ist.",

                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Information);

                return;

            }

            catHybridBody1.set_Name("Profile");



            // neue Skizze im ausgewaehlten geometrischen Set anlegen

            Sketches catSketches1 = catHybridBody1.HybridSketches;

            OriginElements catOriginElements = hsp_catiaPart.Part.OriginElements;

            Reference catReference1 = (Reference)catOriginElements.PlaneYZ;

            hsp_catiaProfil = catSketches1.Add(catReference1);



            // Achsensystem in Skizze erstellen 

            ErzeugeAchsensystem();



            // Part aktualisieren

            hsp_catiaPart.Part.Update();

        }



        private void ErzeugeAchsensystem()

        {

            object[] arr = new object[] {0.0, 0.0, 0.0,

                                         0.0, 1.0, 0.0,

                                         0.0, 0.0, 1.0 };

            hsp_catiaProfil.SetAbsoluteAxisData(arr);

        }



        public void Stirnzahnrad(double aModul, double bZaehne, double dBreite, double eKopf, double hPar, double fFußhoehe, double iTeil, double jFußkr, double kGrndkr, double nKpfkr)

        {



            //Profil erstellen

            //Nullpunkt

            double x0 = 0;

            double y0 = 0;



            //Hilfsgrößen

            double Teilkreisradius = aModul*bZaehne / 2;

            double Hilfskreisradius = Teilkreisradius * 0.94;

            double Fußkreisradius = Teilkreisradius - (1.25 * aModul);

            double Kopfkreisradius = Teilkreisradius + aModul;

            double Verrundungsradius = 0.35 * aModul;



            double Alpha = 20;

            double Beta = 90 / bZaehne;

            double Betarad = Math.PI * Beta / 180;

            double Gamma = 90 - (Alpha - Beta);

            double Gammarad = Math.PI * Gamma / 180;

            double Totalangel = 360.0 / bZaehne;

            double Totalangelrad = Math.PI + Totalangel / 180;



            //Punkte erzeugen

            //Kleiner Kreis

            double xMittelpunktaufEvol_links = Hilfskreisradius * Math.Cos(Gammarad);

            double yMittelpunktaufEvol_links = Hilfskreisradius * Math.Sin(Gammarad);



            //Schnittpunkt auf Evolvente und Teilkreisradius

            double xPunktaufEvolvente = -Teilkreisradius * Math.Sin(Betarad);

            double yPunktaufEvolvente = Teilkreisradius * Math.Cos(Betarad);



            //Evolventenkreis Radius

            double EvolventenkreisRadius = Math.Sqrt(Math.Pow((xMittelpunktaufEvol_links - xPunktaufEvolvente), 2) + Math.Pow((yMittelpunktaufEvol_links - yPunktaufEvolvente), 2));



            //Koordinaten Schnittpunkt Kopfkreis und Evolventenkreis

            double xEvolventenkopfkreis_links = Schnittpunkt_X(x0, y0, Kopfkreisradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius);

            double yEvolventenkopfkreis_links = Schnittpunkt_Y(x0, y0, Kopfkreisradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius);



            //Mittelpunktkoordinaten Verrundung

            double xMittelpunktVerrundung_links = Schnittpunkt_X(x0, y0, Fußkreisradius + Verrundungsradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius + Verrundungsradius);

            double yMittelpunktVerrundung_links = Schnittpunkt_Y(x0, y0, Fußkreisradius + Verrundungsradius, xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius + Verrundungsradius);



            //Schnittpunktkoordinaten Verrundung - Evolventenkreis

            double x_SP_EvolventeVerrundung_links = Schnittpunkt_X(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);

            double y_SP_EvolventeVerrundung_links = Schnittpunkt_Y(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);



            //Schnittpunktkoordinaten Verrundung - Fußkreis

            double x_SP_FußkreisradiusVerrundung_links = Schnittpunkt_X(x0, y0, Fußkreisradius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);

            double y_SP_FußkreisradiusVerrundung_links = Schnittpunkt_Y(x0, y0, Fußkreisradius, xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius);



            //Koordinaten Anfangspunkt Fußkreis

            double Hilfswinkel = Totalangelrad - Math.Atan(Math.Abs(x_SP_FußkreisradiusVerrundung_links) / Math.Abs(y_SP_FußkreisradiusVerrundung_links));

            double x_AnfangspunktFußkreis = -Fußkreisradius * Math.Sin(Hilfswinkel);

            double y_AnfangspunktFußkreis = Fußkreisradius * Math.Cos(Hilfswinkel);



            //Ende



            //Skizze umbenennen

            hsp_catiaProfil.set_Name("Zahnrad");

            Factory2D catfactory2D1 = hsp_catiaProfil.OpenEdition();



            //Punkte in Skizze

            Point2D point_Ursprung = catfactory2D1.CreatePoint(x0, y0);

            Point2D pointAnfangFußkreisLinks = catfactory2D1.CreatePoint(x_AnfangspunktFußkreis, y_AnfangspunktFußkreis);

            Point2D pointFußkreisVerrundungLinks = catfactory2D1.CreatePoint(x_SP_FußkreisradiusVerrundung_links, y_SP_FußkreisradiusVerrundung_links);

            Point2D pointFußkreisVerrundungRechts = catfactory2D1.CreatePoint(-x_SP_FußkreisradiusVerrundung_links, y_SP_FußkreisradiusVerrundung_links);

            Point2D pointMittelpunktVerrundungLinks = catfactory2D1.CreatePoint(xMittelpunktVerrundung_links, yMittelpunktVerrundung_links);

            Point2D pointMittelpunktVerrundungRechts = catfactory2D1.CreatePoint(-xMittelpunktVerrundung_links, yMittelpunktVerrundung_links);

            Point2D pointVerrundungEvolventeLinks = catfactory2D1.CreatePoint(x_SP_EvolventeVerrundung_links, y_SP_EvolventeVerrundung_links);

            Point2D pointVerrundungEvolventeRechts = catfactory2D1.CreatePoint(-x_SP_EvolventeVerrundung_links, y_SP_EvolventeVerrundung_links);

            Point2D pointMittelpunktevolventeLinks = catfactory2D1.CreatePoint(xMittelpunktaufEvol_links, xMittelpunktaufEvol_links);

            Point2D pointMittelpunktevolventeRechts = catfactory2D1.CreatePoint(-xMittelpunktaufEvol_links, yMittelpunktaufEvol_links);

            Point2D pointEvolventenKopfkreisLinks = catfactory2D1.CreatePoint(xEvolventenkopfkreis_links, yEvolventenkopfkreis_links);

            Point2D pointEvolventenKopfkreisRechts = catfactory2D1.CreatePoint(-xEvolventenkopfkreis_links, yEvolventenkopfkreis_links);



            //Kreise

            Circle2D KreisFußkreis = catfactory2D1.CreateCircle(x0, y0, Fußkreisradius, 0, Math.PI * 2);

            KreisFußkreis.CenterPoint = point_Ursprung;

            KreisFußkreis.StartPoint = pointFußkreisVerrundungLinks;

            KreisFußkreis.EndPoint = pointAnfangFußkreisLinks;



            Circle2D KreisVerrundungLinks = catfactory2D1.CreateCircle(xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius, 0, Math.PI * 2);

            KreisVerrundungLinks.CenterPoint = pointMittelpunktVerrundungLinks;

            KreisVerrundungLinks.StartPoint = pointFußkreisVerrundungLinks;

            KreisVerrundungLinks.EndPoint = pointVerrundungEvolventeLinks;



            Circle2D KreisEvolventenkreisLinks = catfactory2D1.CreateCircle(xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, 0, Math.PI * 2);

            KreisEvolventenkreisLinks.CenterPoint = pointMittelpunktevolventeLinks;

            KreisEvolventenkreisLinks.StartPoint = pointEvolventenKopfkreisLinks;

            KreisEvolventenkreisLinks.EndPoint = pointVerrundungEvolventeLinks;



            Circle2D KreisKopfkreis = catfactory2D1.CreateCircle(x0, y0, Kopfkreisradius, 0, Math.PI * 2);

            KreisKopfkreis.CenterPoint = point_Ursprung;

            KreisKopfkreis.StartPoint = pointEvolventenKopfkreisRechts;

            KreisKopfkreis.EndPoint = pointEvolventenKopfkreisLinks;



            Circle2D KreisEvolventenkreisRechts = catfactory2D1.CreateCircle(-xMittelpunktaufEvol_links, yMittelpunktaufEvol_links, EvolventenkreisRadius, 0, Math.PI * 2);

            KreisEvolventenkreisRechts.CenterPoint = pointMittelpunktVerrundungRechts;

            KreisEvolventenkreisRechts.StartPoint = pointVerrundungEvolventeRechts;

            KreisEvolventenkreisRechts.EndPoint = pointEvolventenKopfkreisRechts;



            Circle2D KreisVerrundungRechts = catfactory2D1.CreateCircle(-xMittelpunktVerrundung_links, yMittelpunktVerrundung_links, Verrundungsradius, 0, Math.PI * 2);

            KreisVerrundungRechts.CenterPoint = pointMittelpunktVerrundungRechts;

            KreisVerrundungRechts.StartPoint = pointVerrundungEvolventeRechts;

            KreisVerrundungRechts.EndPoint = pointFußkreisVerrundungRechts;



            //Skizzierer schließen

            hsp_catiaProfil.CloseEdition();



            //Aktualisieren

            hsp_catiaPart.Part.Update();







            //Kreismuster erstellen

            //Deklarierung

            ShapeFactory SF = (ShapeFactory)hsp_catiaPart.Part.ShapeFactory;

            HybridShapeFactory HSF = (HybridShapeFactory)hsp_catiaPart.Part.HybridShapeFactory;

            Part myPart = hsp_catiaPart.Part;

            //Kreismuster mit Parametern füttern

            //Referenzen um die im Muster verwendete Skizze dekraliert werden

            Factory2D Factory2D1 = hsp_catiaProfil.Factory2D;

            HybridShapePointCoord Ursprung = HSF.AddNewPointCoord(0, 0, 0);

            Reference RefUrsprung = myPart.CreateReferenceFromObject(Ursprung);

            HybridShapeDirection XDir = HSF.AddNewDirectionByCoord(1, 0, 0);

            Reference RefXDir = myPart.CreateReferenceFromObject(XDir);



            //Kreismuster mit Daten auffüllen

            CircPattern Kreismuster = SF.AddNewSurfacicCircPattern(Factory2D1, 1, 2, 0, 0, 1, 1, RefUrsprung, RefXDir, false, 0, true, false);

            Kreismuster.CircularPatternParameters = CatCircularPatternParameters.catInstancesandAngularSpacing;

            AngularRepartition angularRepartition1 = Kreismuster.AngularRepartition;

            Angle angle1 = angularRepartition1.AngularSpacing;

            angle1.Value = Convert.ToDouble(360 / Convert.ToDouble(bZaehne));

            AngularRepartition angularRepartition2 = Kreismuster.AngularRepartition;

            IntParam intParam1 = angularRepartition2.InstancesCount;

            intParam1.Value = Convert.ToInt32(bZaehne) + 1;



            //geschlossene Kontur herstellen"Verbindung" oder "Join"

            Reference Ref_Kreismuster = myPart.CreateReferenceFromObject(Kreismuster);

            HybridShapeAssemble Verbindung = HSF.AddNewJoin(Ref_Kreismuster, Ref_Kreismuster);

            Reference Ref_Verbindung = myPart.CreateReferenceFromObject(Verbindung);

            HSF.GSMVisibility(Ref_Verbindung, 0);

            myPart.Update();

            Bodies bodies = myPart.Bodies;

            Body myBody = bodies.Add();

            myBody.set_Name("Zahnrad");

            myBody.InsertHybridShape(Verbindung);

            myPart.Update();



        }



        public void Dicke()

        {

            //3D-Modell erzeugen

            hsp_catiaPart.Part.InWorkObject = hsp_catiaPart.Part.MainBody;



        }







        private double Schnittpunkt_X(double xMittelpunkt, double yMittelpunkt, double Radius1, double xMittelpunkt2, double yMittelpunkt2, double Radius2)

        {

            double d = Math.Sqrt(Math.Pow((xMittelpunkt - xMittelpunkt2), 2) + Math.Pow((yMittelpunkt - yMittelpunkt2), 2));

            double l = (Math.Pow(Radius1, 2) - Math.Pow(d, 2)) / (d * 2);

            double h;

            double Verbindungsabfrage = 0.00001;



            if (Radius1 - 1 < -Verbindungsabfrage)

            {

                MessageBox.Show("Fehler Verbindungsabfrage");

            }

            if (Math.Abs(Radius1 - 1) < Verbindungsabfrage)

            {

                h = 0;

            }

            else

            {

                h = Math.Sqrt(Math.Pow(Radius1, 2) - Math.Pow(1, 2));

            }



            return 1 * (xMittelpunkt2 - xMittelpunkt) / d - h * (yMittelpunkt2 - yMittelpunkt) / d + xMittelpunkt;

        }





        private double Schnittpunkt_Y(double xMittelpunkt, double yMittelpunkt, double Radius1, double xMittelpunkt2, double yMittelpunkt2, double Radius2)

        {

            double d = Math.Sqrt(Math.Pow((xMittelpunkt - xMittelpunkt2), 2) + Math.Pow((yMittelpunkt - yMittelpunkt2), 2));

            double l = (Math.Pow(Radius1, 2) - Math.Pow(Radius2, 2) + Math.Pow(d, 2)) / (d * 2);

            double h;

            double Verbindungsabfrage = 0.00001;



            if (Radius1 - 1 < -Verbindungsabfrage)

            {

                MessageBox.Show("Fehler Verbindungsabfrage 2");

            }

            if (Math.Abs(Radius1 - 1) < Verbindungsabfrage)

            {

                h = 0;

            }

            else

            {

                h = Math.Sqrt(Math.Pow(Radius1, 2) - Math.Pow(1, 2));

            }



            return 1 * (yMittelpunkt2 - yMittelpunkt) / d - h * (xMittelpunkt2 - xMittelpunkt) / d + yMittelpunkt;

        }

    }

}