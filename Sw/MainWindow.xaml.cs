using Sw.Cls;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Sw
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();

            //Tratamiento COMBOS
            cmbTipo.Items.Add("Seleccione");
            cmbTipo.SelectedIndex = 0;
            // Stat Principal + 5 Sub-Stats
            Principal.Items.Add("Seleccione");
            Principal.SelectedIndex = 0;
            Sub1.Items.Add("Seleccione");
            Sub1.SelectedIndex = 0;
            Sub2.Items.Add("Seleccione");
            Sub2.SelectedIndex = 0;
            Sub3.Items.Add("Seleccione");
            Sub3.SelectedIndex = 0;
            Sub4.Items.Add("Seleccione");
            Sub4.SelectedIndex = 0;
            Sub5.Items.Add("Seleccione");
            Sub5.SelectedIndex = 0;

            //No se puede escribir en un stat, si no se ha seleccionado el tipo
            txtPrincipal.IsEnabled = false;
            txtSub1.IsEnabled = false;
            txtSub2.IsEnabled = false;
            txtSub3.IsEnabled = false;
            txtSub4.IsEnabled = false;
            txtSub5.IsEnabled = false;


            StatCR.Content = " +15%";
            StatCD.Content = " +50%";
            StatRES.Content = " +15%";
            StatACC.Content = " +0%";


            //Limpia el lblEstado 5 segundos despues de haber mostrado un mensaje
            Basetemplate.Template = rune1.Template;
            dis.Interval = new TimeSpan(0, 0, 5);
            dis.Tick += (s, a) =>
            {
                lblEstado.Content = "";
                dis.Stop();
            };


        }

        //Timer creado para limpiar el mensaje de error 5 segundos despues de ser mostrado
        DispatcherTimer dis = new DispatcherTimer();
        int Slot = 0;
        Button Basetemplate = new Button();


        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {

            //Debe seleccionar un slot
            if (Slot > 0)
            {
                try
                {
                    //Cada vez que se guarda una runa, el slot debe estar vacio
                    foreach (var item in App.Runas)
                    {
                        if (Slot + "" == item.Slot)
                        {
                            int index = App.Runas.FindIndex(i => i.Slot.Equals("" + Slot));
                            App.Runas.RemoveAt(index);
                            break;
                        }
                    }

                    if (Principal.Text == "Seleccione")
                    {
                        lblEstado.Content = "Debe seleccionar el Stat principal";
                        dis.Start();
                    }
                    else
                    {
                        lblEstado.Content = "Runa " + Slot + " almacenada";
                        dis.Start();
                        GuardarEnLista();
                    }



                }
                catch (Exception a)
                {
                    MessageBox.Show(a + "");
                }
            }
            else
            {
                lblEstado.Content = "Seleccione Slot";
                dis.Start();

            }

        }


        public void GuardarEnLista()
        {
            Runa nuevaRuna = new Runa();

            nuevaRuna.Slot = Slot + "";
            nuevaRuna.Tipo = cmbTipo.Text;

            nuevaRuna.Principal = Principal.Text;
            nuevaRuna.Sub1 = Sub1.Text;
            nuevaRuna.Sub2 = Sub2.Text;
            nuevaRuna.Sub3 = Sub3.Text;
            nuevaRuna.Sub4 = Sub4.Text;
            nuevaRuna.Sub5 = Sub5.Text;

            nuevaRuna.StatPrincipal = txtPrincipal.Text;
            nuevaRuna.StatSub1 = txtSub1.Text;
            nuevaRuna.StatSub2 = txtSub2.Text;
            nuevaRuna.StatSub3 = txtSub3.Text;
            nuevaRuna.StatSub4 = txtSub4.Text;
            nuevaRuna.StatSub5 = txtSub5.Text;



            App.Runas.Add(nuevaRuna);

            Calculos();

        }


        //STATS BASE MOB
        string HP = "";
        string ATK = "";
        string DEF = "";
        string VEL = "";
        string CD = "";
        string CR = "";
        string RES = "";
        string ACC = "";
        //STAT TOTALES (" +x ")
        double HPTotal = 0;
        double ATKTotal = 0;
        double DEFTotal = 0;
        int VELTotal = 0;
        int CDTotal = 50;
        int CRTotal = 15;
        int RESTotal = 15;
        int ACCTotal = 0;

        //Contadores
        int cEnergy = 0;
        int cFatal = 0;
        int cBlade = 0;
        int cSwift = 0;
        int cFocus = 0;
        int cRage = 0;
        int cShield = 0;
        int cRevenge = 0;
        int cWill = 0;
        int cNemesis = 0;
        int cVampire = 0;
        int cDestroy = 0;
        int cDespair = 0;
        int cViolent = 0;
        int cFight = 0;
        int cDetermination = 0;
        int cEnhance = 0;
        int cAccuracy = 0;
        int cTolerance = 0;
        int cGuard = 0;
        int cEndure = 0;


        public void Calculos()
        {
            //Se encarga de limpiar los valores de los stat de los mobs para hacer los calculos porcentuales
            PrepararValoresPorcentuales();
            //Sumar el bonus otorgado por el set         
            CalcularSet();
            //Calcular STATS 
            try
            {
                foreach (var item in App.Runas)
                {
                    //Los sgtes arreglos solo están hechos para ahorrar codigo.
                    //En cada parte de este arreglo se guardará el tipo de stat, el cual se filtrará en el switch
                    string[] TipoStat = new string[6];
                    TipoStat[0] = item.Principal;
                    TipoStat[1] = item.Sub1;
                    TipoStat[2] = item.Sub2;
                    TipoStat[3] = item.Sub3;
                    TipoStat[4] = item.Sub4;
                    TipoStat[5] = item.Sub5;
                    //En cada parte de este arreglo se guardará el valor del stat, el cual será usado para calcular el total
                    string[] StatRuna = new string[6];
                    StatRuna[0] = item.StatPrincipal;
                    StatRuna[1] = item.StatSub1;
                    StatRuna[2] = item.StatSub2;
                    StatRuna[3] = item.StatSub3;
                    StatRuna[4] = item.StatSub4;
                    StatRuna[5] = item.StatSub5;


                    for (int i = 0; i < TipoStat.Length; i++)
                    {
                        switch (TipoStat[i])
                        {
                            case "HP":
                                // El metodo CalcularTotal necesita el Valor del Stat. Si dicho valor es porcentual, se le debe pasar el stat del mob. 
                                //Además, se le debe dar un bool (true o false) para que siga alguno de los 2 caminos
                                HPTotal += double.Parse(CalcularTotal(StatRuna[i], HP, true).ToString());
                                break;
                            case "ATK":
                                ATKTotal += double.Parse(CalcularTotal(StatRuna[i], ATK, true).ToString());
                                break;
                            case "DEF":
                                DEFTotal += double.Parse(CalcularTotal(StatRuna[i], DEF, true).ToString());
                                break;
                            case "VEL":
                                if (StatRuna[i].Length == 0)
                                {
                                    StatRuna[i] = "0";
                                }
                                VELTotal += int.Parse(StatRuna[i]);
                                break;
                            case "CRI. Rate":
                                CRTotal += int.Parse(CalcularTotal(StatRuna[i], CR, false).ToString());
                                break;
                            case "CRI. Dmg":
                                CDTotal += int.Parse(CalcularTotal(StatRuna[i], CD, false).ToString());
                                break;
                            case "RES":
                                RESTotal += int.Parse(CalcularTotal(StatRuna[i], RES, false).ToString());
                                break;
                            case "ACC":
                                ACCTotal += int.Parse(CalcularTotal(StatRuna[i], ACC, false).ToString());
                                break;
                        }
                    }
                }


                double CRFinal = 0;
                double CDFinal = 0;
                double RESFinal = 0;
                double ACCFinal = 0;
                CRFinal = CRTotal + int.Parse(CR);
                CDFinal = CDTotal + int.Parse(CD);
                RESFinal = RESTotal + int.Parse(RES);
                ACCFinal = ACCTotal + int.Parse(ACC);






                if (StatHP.Content.ToString() != " +0")
                {
                    StatHP.Content = " +" + (Math.Round(HPTotal));
                }
                else
                {
                    StatHP.Content = " +" + (Math.Round(HPTotal));
                }
                if (StatATK.Content.ToString() != " +0")
                {
                    StatATK.Content = " +" + (Math.Round(ATKTotal));
                }
                else
                {
                    StatATK.Content = " +" + (Math.Round(ATKTotal));
                }

                if (StatDEF.Content.ToString() != " +0")
                {
                    StatDEF.Content = " +" + (Math.Round(DEFTotal));
                }
                else
                {
                    StatDEF.Content = " +" + (Math.Round(DEFTotal));
                }

                StatVEL.Content = " +" + VELTotal;
                StatCR.Content = " +" + CRFinal + "%";
                StatCD.Content = " +" + CDFinal + "%";
                StatRES.Content = " +" + RESFinal + "%";
                StatACC.Content = " +" + ACCFinal + "%";


            }
            catch (Exception a)
            {
                MessageBox.Show(a + "");
            }
            finally
            {

                HPTotal = 0;
                ATKTotal = 0;
                DEFTotal = 0;
                VELTotal = 0;
                CDTotal = 50;
                CRTotal = 15;
                RESTotal = 15;
                ACCTotal = 0;

            }


        }

        public void PrepararValoresPorcentuales()
        {
            //HP
            if (txtHP2.Text.Length != 0)
            {
                HP = txtHP2.Text;
            }
            else
            {
                HP = "0";
            }
            //ATK
            if (txtATK2.Text.Length != 0)
            {
                ATK = txtATK2.Text;
            }
            else
            {
                ATK = "0";
            }
            //DEF
            if (txtDEF2.Text.Length != 0)
            {
                DEF = txtDEF2.Text;
            }
            else
            {
                DEF = "0";
            }
            //VEL
            if (txtVEL2.Text.Length != 0)
            {
                VEL = txtVEL2.Text;
            }
            else
            {
                VEL = "0";
            }
            //CD
            if (txtCD2.Text.Length >= 1 && txtCD2.Text.IndexOf("%") >= 0)
            {
                CD = txtCD2.Text.Substring(0, txtCD2.Text.IndexOf("%"));
            }
            else
            {
                if (txtCD2.Text.Length > 0)
                {
                    CD = txtCD2.Text;
                }
                else
                {
                    CD = "0";
                }
            }
            //CR
            if (txtCR2.Text.Length >= 1 && txtCR2.Text.IndexOf("%") >= 0)
            {
                CR = txtCR2.Text.Substring(0, txtCR2.Text.IndexOf("%"));
            }
            else
            {
                if (txtCR2.Text.Length != 0)
                {
                    CR = txtCR2.Text;
                }
                else
                {
                    CR = "0";
                }
            }
            //RES
            if (txtRES2.Text.Length >= 1 && txtRES2.Text.IndexOf("%") >= 0)
            {
                RES = txtRES2.Text.Substring(0, txtRES2.Text.IndexOf("%"));
            }
            else
            {
                if (txtRES2.Text.Length != 0)
                {
                    RES = txtRES2.Text;
                }
                else
                {
                    RES = "0";
                }
            }
            if (txtACC2.Text.Length >= 1 && txtACC2.Text.IndexOf("%") >= 0)
            {
                ACC = txtACC2.Text.Substring(0, txtACC2.Text.IndexOf("%"));
            }
            else
            {
                if (txtACC2.Text.Length != 0)
                {
                    ACC = txtACC2.Text;
                }
                else
                {
                    ACC = "0";
                }
            }
        }
        public void CalcularSet()
        {
            Set(true, "", 0);

            try
            {
                foreach (var item in App.Runas)
                {
                    switch (item.Tipo)
                    {
                        case "Energy":
                            cEnergy++;
                            break;
                        case "Fatal":
                            cFatal++;
                            break;
                        case "Blade":
                            cBlade++;
                            break;
                        case "Swift":
                            cSwift++;
                            break;
                        case "Focus":
                            cFocus++;
                            break;
                        case "Guard":
                            cGuard++;
                            break;
                        case "Endure":
                            cEndure++;
                            break;
                        case "Shield":
                            cShield++;
                            break;
                        case "Revenge":
                            cRevenge++;
                            break;
                        case "Will":
                            cWill++;
                            break;
                        case "Nemesis":
                            cNemesis++;
                            break;
                        case "Vampire":
                            cVampire++;
                            break;
                        case "Destroy":
                            cDestroy++;
                            break;
                        case "Despair":
                            cDespair++;
                            break;
                        case "Violent":
                            cViolent++;
                            break;
                        case "Rage":
                            cRage++;
                            break;
                        case "Fight":
                            cFight++;
                            break;
                        case "Determination":
                            cDetermination++;
                            break;
                        case "Enhance":
                            cEnhance++;
                            break;
                        case "Accuracy":
                            cAccuracy++;
                            break;
                        case "Tolerance":
                            cTolerance++;
                            break;
                    }

                }

                if (cEnergy >= 2)
                {
                    HPTotal += int.Parse(HP) * (0.15 * (cEnergy / 2));
                    Set(true, "Energy", cEnergy);
                }
                if (cFatal >= 4)
                {
                    ATKTotal += int.Parse(ATK) * (0.35);
                    Set(false, "Fatal", cFatal);
                }

                if (cBlade >= 2)
                {
                    CRTotal += 12 * (cBlade / 2);
                    Set(true, "Blade", cBlade);
                }
                if (cSwift >= 4)
                {
                    string vel = "" + (int.Parse(VEL) * 0.25);
                    VELTotal += int.Parse(vel);
                    Set(false, "Swift", cSwift);
                }
                if (cFocus >= 2)
                {
                    ACCTotal += 20 * (cFocus / 2);
                    Set(true, "Focus", cFocus);
                }
                if (cGuard >= 2)
                {
                    DEFTotal += int.Parse(DEF) * (0.2 * (cGuard / 2));
                    Set(true, "Guard", cGuard);
                }
                if (cEndure >= 2)
                {
                    RESTotal += 20 * (cEndure / 2);
                    Set(true, "Endure", cEndure);
                }
                if (cShield >= 2)
                {
                    Set(true, "Shield", cShield);
                }
                if (cRevenge >= 2)
                {
                    Set(true, "Revenge", cRevenge);
                }
                if (cWill >= 2)
                {
                    Set(true, "Will", cWill);
                }
                if (cNemesis >= 2)
                {
                    Set(true, "Nemesis", cNemesis);
                }
                if (cVampire >= 4)
                {
                    Set(false, "Vampire", cVampire);
                }
                if (cDestroy >= 2)
                {
                    Set(true, "Destroy", cDestroy);
                }
                if (cDespair >= 4)
                {
                    Set(false, "Despair", cDespair);
                }
                if (cViolent >= 4)
                {
                    Set(false, "Violent", cViolent);
                }
                if (cRage >= 4)
                {
                    CDTotal += 12;
                    Set(false, "Rage", cRage);
                }
                if (cFight >= 2)
                {
                    Set(true, "Fight", cFight);
                }
                if (cDetermination >= 2)
                {
                    Set(true, "Determination", cDetermination);
                }
                if (cEnhance >= 2)
                {
                    Set(true, "Enhance", cEnhance);
                }
                if (cAccuracy >= 2)
                {
                    Set(true, "Accuracy", cAccuracy);
                }
                if (cTolerance >= 2)
                {
                    Set(true, "Tolerance", cTolerance);
                }


            }
            catch
            {

            }
            finally
            {
                string[] S = new string[3];
                S = Sets;


                ImgSet1.Source = null;
                ImgSet2.Source = null;
                ImgSet3.Source = null;

                if (string.IsNullOrEmpty(S[0]) == false)
                {
                    ImgSet1.Source = new BitmapImage(new Uri(@"../../Img/Rune_type/" + S[0] + ".png", UriKind.Relative));
                }
                if (string.IsNullOrEmpty(S[1]) == false)
                {
                    ImgSet2.Source = new BitmapImage(new Uri(@"../../Img/Rune_type/" + S[1] + ".png", UriKind.Relative));
                }
                if (string.IsNullOrEmpty(S[2]) == false)
                {
                    ImgSet3.Source = new BitmapImage(new Uri(@"../../Img/Rune_type/" + S[2] + ".png", UriKind.Relative));
                }


                cEnergy = 0;
                cFatal = 0;
                cBlade = 0;
                cSwift = 0;
                cFocus = 0;
                cRage = 0;
                cShield = 0;
                cRevenge = 0;
                cWill = 0;
                cNemesis = 0;
                cVampire = 0;
                cDestroy = 0;
                cDespair = 0;
                cViolent = 0;
                cFight = 0;
                cDetermination = 0;
                cEnhance = 0;
                cAccuracy = 0;
                cTolerance = 0;
                cGuard = 0;
                cEndure = 0;
            }
        }

        public string[] Sets = new string[3];
        //Agrega las imagenes para cada set activo 
        //S2 = La runa es Set2
        //Tipo = El tipo de runa (Energy, Fatal, etc).
        //N = Cantidad de runas con ese set
        //La idea de esto es no tener que repetir el codigo
        public void Set(bool S2, string Tipo, int N)
        {
            Sets = new string[3];
            if (S2)
            {
                if (N == 6)
                {
                    Sets[0] = Tipo;
                    Sets[1] = Tipo;
                    Sets[2] = Tipo;
                }

                if (N >= 4 && N < 6)
                {
                    if (string.IsNullOrEmpty(Sets[0]))
                    {
                        Sets[0] = Tipo;

                        if (string.IsNullOrEmpty(Sets[1]))
                        {
                            Sets[1] = Tipo;
                        }
                        else
                        {
                            Sets[2] = Tipo;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Sets[1]))
                        {
                            Sets[1] = Tipo;
                        }
                        else
                        {
                            Sets[2] = Tipo;
                        }
                    }
                }

                if (N >= 2 && N < 4)
                {
                    if (string.IsNullOrEmpty(Sets[0]))
                    {
                        Sets[0] = Tipo;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Sets[1]))
                        {
                            Sets[1] = Tipo;
                        }
                        else
                        {
                            Sets[2] = Tipo;
                        }
                    }
                }

            }
            else
            {
                if (string.IsNullOrEmpty(Sets[0]))
                {
                    Sets[0] = Tipo;

                }
                else
                {
                    Sets[1] = Tipo;
                    Sets[2] = "";
                }
            }

        }

        //Se encarga de calcular el valor total del stat 
        //Valor es el valor del stat y OBJ es el campo en el cual se saca el stat del mob 
        public double CalcularTotal(string VALOR, string OBJ, bool tipo)
        {

            if (VALOR.Length == 0)
            {
                VALOR = "0";
            }

            double stat = 0;

            double aux = 0.0;
            if (tipo)
            {
                if (VALOR.IndexOf("%") != -1)
                {
                    stat = double.Parse(VALOR.Substring(0, VALOR.Length - 1));
                    aux = stat / 100.0;
                    return double.Parse((int.Parse(OBJ) * aux).ToString());
                }
                else
                {
                    return double.Parse(VALOR);
                }
            }
            else
            {
                if (VALOR.IndexOf("%") != -1)
                {
                    VALOR = VALOR.Substring(0, VALOR.IndexOf("%"));
                }
                return int.Parse(VALOR);
            }
        }



        public void Controles(int slot)
        {
            //Limpiar
            cmbTipo.Items.Clear();
            cmbTipo.Items.Add("Seleccione");
            cmbTipo.SelectedIndex = 0;

            Principal.Items.Clear();
            Principal.Items.Add("Seleccione");
            Principal.SelectedIndex = 0;

            Sub1.Items.Clear();
            Sub1.Items.Add("Seleccione");
            Sub1.SelectedIndex = 0;

            Sub2.Items.Clear();
            Sub2.Items.Add("Seleccione");
            Sub2.SelectedIndex = 0;

            Sub3.Items.Clear();
            Sub3.Items.Add("Seleccione");
            Sub3.SelectedIndex = 0;

            Sub4.Items.Clear();
            Sub4.Items.Add("Seleccione");
            Sub4.SelectedIndex = 0;

            Sub5.Items.Clear();
            Sub5.Items.Add("Seleccione");
            Sub5.SelectedIndex = 0;

            txtPrincipal.Text = "";
            txtSub1.Text = "";
            txtSub2.Text = "";
            txtSub3.Text = "";
            txtSub4.Text = "";
            txtSub5.Text = "";

            txtPrincipal.IsEnabled = false;
            txtSub1.IsEnabled = false;
            txtSub2.IsEnabled = false;
            txtSub3.IsEnabled = false;
            txtSub4.IsEnabled = false;
            txtSub5.IsEnabled = false;


            //Llenar            
            foreach (var item in App.Runas)
            {
                if (slot + "" == item.Slot)
                {
                    cmbTipo.Items.Add(item.Tipo);
                    cmbTipo.SelectedIndex = 1;
                    Principal.Items.Add(item.Principal);
                    Principal.SelectedIndex = 1;
                    Sub1.Items.Add(item.Sub1);
                    Sub1.SelectedIndex = 1;
                    Sub2.Items.Add(item.Sub2);
                    Sub2.SelectedIndex = 1;
                    Sub3.Items.Add(item.Sub3);
                    Sub3.SelectedIndex = 1;
                    Sub4.Items.Add(item.Sub4);
                    Sub4.SelectedIndex = 1;
                    Sub5.Items.Add(item.Sub5);
                    Sub5.SelectedIndex = 1;

                    if (Principal.Text != "Seleccione")
                    {
                        txtPrincipal.IsEnabled = true;
                    }
                    if (Sub1.Text != "Seleccione")
                    {
                        txtSub1.IsEnabled = true;
                    }
                    if (Sub2.Text != "Seleccione")
                    {
                        txtSub2.IsEnabled = true;
                    }
                    if (Sub3.Text != "Seleccione")
                    {
                        txtSub3.IsEnabled = true;
                    }
                    if (Sub4.Text != "Seleccione")
                    {
                        txtSub4.IsEnabled = true;
                    }
                    if (Sub5.Text != "Seleccione")
                    {
                        txtSub5.IsEnabled = true;
                    }

                    txtPrincipal.Text = item.StatPrincipal;
                    txtSub1.Text = item.StatSub1;
                    txtSub2.Text = item.StatSub2;
                    txtSub3.Text = item.StatSub3;
                    txtSub4.Text = item.StatSub4;
                    txtSub5.Text = item.StatSub5;
                }
            }

        }

        //Btn Limpiar
        private void btnCLS_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (Slot > 0)
                {


                    foreach (var item in App.Runas)
                    {
                        if (Slot + "" == item.Slot)
                        {
                            int index = App.Runas.FindIndex(i => i.Slot.Equals("" + Slot));
                            App.Runas.RemoveAt(index);
                            break;

                        }

                    }

                    cmbTipo.Items.Clear();
                    cmbTipo.Items.Add("Seleccione");
                    cmbTipo.SelectedIndex = 0;

                    txtPrincipal.Text = "";
                    txtSub1.Text = "";
                    txtSub2.Text = "";
                    txtSub3.Text = "";
                    txtSub4.Text = "";
                    txtSub5.Text = "";

                    Calculos();

                }
                else
                {
                    lblEstado.Content = "Seleccione Slot";
                    dis.Start();
                }
            }
            catch (Exception A)
            {
                MessageBox.Show("" + A);
            }
        }

        //BTN_Runas

        private void rune1_Click(object sender, RoutedEventArgs e)
        {
            lblSlot.Content = "Slot: 1";
            Slot = 1;

            rune2.Template = null;
            rune3.Template = null;
            rune4.Template = null;
            rune5.Template = null;
            rune6.Template = null;

            rune2.Template = Basetemplate.Template;
            rune3.Template = Basetemplate.Template;
            rune4.Template = Basetemplate.Template;
            rune5.Template = Basetemplate.Template;
            rune6.Template = Basetemplate.Template;


            Image image = (Image)rune1.Template.FindName("image1", rune1);
            image.Source = new BitmapImage(new Uri(@"../../Img/onClick_rune.png", UriKind.Relative));
            image.OnApplyTemplate();


            Controles(Slot);

            Principal.Items.Clear();
            Principal.Items.Add("ATK");
            Principal.SelectedIndex = 0;
            Principal.IsEnabled = false;
            txtPrincipal.IsEnabled = true;
        }

        private void rune2_Click(object sender, RoutedEventArgs e)
        {
            lblSlot.Content = "Slot: 2";
            Slot = 2;


            rune1.Template = null;
            rune3.Template = null;
            rune4.Template = null;
            rune5.Template = null;
            rune6.Template = null;

            rune1.Template = Basetemplate.Template;
            rune3.Template = Basetemplate.Template;
            rune4.Template = Basetemplate.Template;
            rune5.Template = Basetemplate.Template;
            rune6.Template = Basetemplate.Template;



            Image image = (Image)rune2.Template.FindName("image1", rune2);
            image.Source = new BitmapImage(new Uri(@"../../Img/onClick_rune.png", UriKind.Relative));
            image.OnApplyTemplate();


            Controles(Slot);
            Principal.IsEnabled = true;
        }

        private void rune3_Click(object sender, RoutedEventArgs e)
        {



            lblSlot.Content = "Slot: 3";
            Slot = 3;

            rune1.Template = null;
            rune2.Template = null;
            rune4.Template = null;
            rune5.Template = null;
            rune6.Template = null;

            rune1.Template = Basetemplate.Template;
            rune2.Template = Basetemplate.Template;
            rune4.Template = Basetemplate.Template;
            rune5.Template = Basetemplate.Template;
            rune6.Template = Basetemplate.Template;

            Image image = (Image)rune3.Template.FindName("image1", rune3);
            image.Source = new BitmapImage(new Uri(@"../../Img/onClick_rune.png", UriKind.Relative));
            image.OnApplyTemplate();

            Controles(Slot);

            Principal.Items.Clear();
            Principal.Items.Add("DEF");
            Principal.SelectedIndex = 0;
            Principal.IsEnabled = false;
            txtPrincipal.IsEnabled = true;

        }

        private void rune4_Click(object sender, RoutedEventArgs e)
        {
            lblSlot.Content = "Slot: 4";
            Slot = 4;

            rune1.Template = null;
            rune2.Template = null;
            rune3.Template = null;
            rune5.Template = null;
            rune6.Template = null;

            rune1.Template = Basetemplate.Template;
            rune2.Template = Basetemplate.Template;
            rune3.Template = Basetemplate.Template;
            rune5.Template = Basetemplate.Template;
            rune6.Template = Basetemplate.Template;

            Image image = (Image)rune4.Template.FindName("image1", rune4);
            image.Source = new BitmapImage(new Uri(@"../../Img/onClick_rune.png", UriKind.Relative));
            image.OnApplyTemplate();

            Controles(Slot);
            Principal.IsEnabled = true;
        }

        private void rune5_Click(object sender, RoutedEventArgs e)
        {
            lblSlot.Content = "Slot: 5";
            Slot = 5;

            rune1.Template = null;
            rune2.Template = null;
            rune3.Template = null;
            rune4.Template = null;
            rune6.Template = null;

            rune1.Template = Basetemplate.Template;
            rune2.Template = Basetemplate.Template;
            rune3.Template = Basetemplate.Template;
            rune4.Template = Basetemplate.Template;
            rune6.Template = Basetemplate.Template;

            Image image = (Image)rune5.Template.FindName("image1", rune5);
            image.Source = new BitmapImage(new Uri(@"../../Img/onClick_rune.png", UriKind.Relative));
            image.OnApplyTemplate();

            Controles(Slot);

            Principal.Items.Clear();
            Principal.Items.Add("HP");
            Principal.SelectedIndex = 0;
            Principal.IsEnabled = false;
            txtPrincipal.IsEnabled = true;
        }

        private void rune6_Click(object sender, RoutedEventArgs e)
        {
            lblSlot.Content = "Slot: 6";
            Slot = 6;

            rune1.Template = null;
            rune2.Template = null;
            rune3.Template = null;
            rune4.Template = null;
            rune5.Template = null;

            rune1.Template = Basetemplate.Template;
            rune2.Template = Basetemplate.Template;
            rune3.Template = Basetemplate.Template;
            rune4.Template = Basetemplate.Template;
            rune5.Template = Basetemplate.Template;

            Image image = (Image)rune6.Template.FindName("image1", rune6);
            image.Source = new BitmapImage(new Uri(@"../../Img/onClick_rune.png", UriKind.Relative));
            image.OnApplyTemplate();

            Controles(Slot);
            Principal.IsEnabled = true;
        }


        //Tratamiento combos

        private void cmbTipo_DropDownOpened(object sender, EventArgs e)
        {
            int SelectIndex = cmbTipo.SelectedIndex;

            cmbTipo.Items.Clear();
            cmbTipo.Items.Add("Energy");
            cmbTipo.Items.Add("Blade");
            cmbTipo.Items.Add("Destroy");
            cmbTipo.Items.Add("Despair");
            cmbTipo.Items.Add("Endure");
            cmbTipo.Items.Add("Fatal");
            cmbTipo.Items.Add("Focus");
            cmbTipo.Items.Add("Nemesis");
            cmbTipo.Items.Add("Rage");
            cmbTipo.Items.Add("Revenge");
            cmbTipo.Items.Add("Shield");
            cmbTipo.Items.Add("Swift");
            cmbTipo.Items.Add("Vampire");
            cmbTipo.Items.Add("Will");
            cmbTipo.Items.Add("Fight");
            cmbTipo.Items.Add("Determination");
            cmbTipo.Items.Add("Enhance");
            cmbTipo.Items.Add("Accuracy");
            cmbTipo.Items.Add("Tolerance");
            cmbTipo.SelectedIndex = SelectIndex;
        }

        private void cmbTipo_DropDownClosed(object sender, EventArgs e)
        {
            if (cmbTipo.Text == "")
            {
                cmbTipo.Items.Clear();
                cmbTipo.Items.Add("Seleccione");
                cmbTipo.SelectedIndex = 0;
            }

        }

        private void Principal_DropDownOpened(object sender, EventArgs e)
        {
            int SelectIndex = Principal.SelectedIndex;


            switch (Slot)
            {
                case 2:
                    Principal.Items.Clear();
                    Principal.Items.Add("Vacio");
                    Principal.Items.Add("HP");
                    Principal.Items.Add("ATK");
                    Principal.Items.Add("DEF");
                    Principal.Items.Add("VEL");
                    Principal.SelectedIndex = SelectIndex;
                    break;
                case 4:
                    Principal.Items.Clear();
                    Principal.Items.Add("Vacio");
                    Principal.Items.Add("HP");
                    Principal.Items.Add("ATK");
                    Principal.Items.Add("DEF");
                    Principal.Items.Add("CRI. Rate");
                    Principal.Items.Add("CRI. Dmg");
                    Principal.SelectedIndex = SelectIndex;
                    break;
                case 6:
                    Principal.Items.Clear();
                    Principal.Items.Add("Vacio");
                    Principal.Items.Add("HP");
                    Principal.Items.Add("ATK");
                    Principal.Items.Add("DEF");
                    Principal.Items.Add("RES");
                    Principal.Items.Add("ACC");
                    break;

            }
        }

        private void Principal_DropDownClosed(object sender, EventArgs e)
        {
            if (Principal.Text == "" || Principal.Text == "Vacio")
            {
                txtPrincipal.IsEnabled = false;
                Principal.Items.Clear();
                Principal.Items.Add("Seleccione");
                Principal.SelectedIndex = 0;
            }
            else
            {
                txtPrincipal.IsEnabled = true;
            }
        }

        private void Sub1_DropDownOpened(object sender, EventArgs e)
        {
            int SelectIndex = Sub1.SelectedIndex;

            Sub1.Items.Clear();
            Sub1.Items.Add("Vacio");
            Sub1.Items.Add("HP");
            Sub1.Items.Add("ATK");
            Sub1.Items.Add("DEF");
            Sub1.Items.Add("VEL");
            Sub1.Items.Add("CRI. Rate");
            Sub1.Items.Add("CRI. Dmg");
            Sub1.Items.Add("RES");
            Sub1.Items.Add("ACC");
            Sub1.SelectedIndex = SelectIndex;
        }

        private void Sub1_DropDownClosed(object sender, EventArgs e)
        {
            if (Sub1.Text == "" || Sub1.Text == "Vacio")
            {
                txtSub1.IsEnabled = false;
                Sub1.Items.Clear();
                Sub1.Items.Add("Seleccione");
                Sub1.SelectedIndex = 0;
            }
            else
            {
                txtSub1.IsEnabled = true;
            }
        }

        private void Sub2_DropDownOpened(object sender, EventArgs e)
        {
            int SelectIndex = Sub2.SelectedIndex;

            Sub2.Items.Clear();
            Sub2.Items.Add("Vacio");
            Sub2.Items.Add("HP");
            Sub2.Items.Add("ATK");
            Sub2.Items.Add("DEF");
            Sub2.Items.Add("VEL");
            Sub2.Items.Add("CRI. Rate");
            Sub2.Items.Add("CRI. Dmg");
            Sub2.Items.Add("RES");
            Sub2.Items.Add("ACC");
            Sub2.SelectedIndex = SelectIndex;
        }

        private void Sub2_DropDownClosed(object sender, EventArgs e)
        {
            if (Sub2.Text == "" || Sub2.Text == "Vacio")
            {
                txtSub2.IsEnabled = false;
                Sub2.Items.Clear();
                Sub2.Items.Add("Seleccione");
                Sub2.SelectedIndex = 0;
            }
            else
            {
                txtSub2.IsEnabled = true;
            }

        }

        private void Sub3_DropDownOpened(object sender, EventArgs e)
        {
            int SelectIndex = Sub3.SelectedIndex;

            Sub3.Items.Clear();
            Sub3.Items.Add("Vacio");
            Sub3.Items.Add("HP");
            Sub3.Items.Add("ATK");
            Sub3.Items.Add("DEF");
            Sub3.Items.Add("VEL");
            Sub3.Items.Add("CRI. Rate");
            Sub3.Items.Add("CRI. Dmg");
            Sub3.Items.Add("RES");
            Sub3.Items.Add("ACC");
            Sub3.SelectedIndex = SelectIndex;
        }

        private void Sub3_DropDownClosed(object sender, EventArgs e)
        {
            if (Sub3.Text == "" || Sub3.Text == "Vacio")
            {
                txtSub3.IsEnabled = false;
                Sub3.Items.Clear();
                Sub3.Items.Add("Seleccione");
                Sub3.SelectedIndex = 0;
            }
            else
            {
                txtSub3.IsEnabled = true;
            }
        }

        private void Sub4_DropDownOpened(object sender, EventArgs e)
        {
            int SelectIndex = Sub4.SelectedIndex;

            Sub4.Items.Clear();
            Sub4.Items.Add("Vacio");
            Sub4.Items.Add("HP");
            Sub4.Items.Add("ATK");
            Sub4.Items.Add("DEF");
            Sub4.Items.Add("VEL");
            Sub4.Items.Add("CRI. Rate");
            Sub4.Items.Add("CRI. Dmg");
            Sub4.Items.Add("RES");
            Sub4.Items.Add("ACC");
            Sub4.SelectedIndex = SelectIndex;
        }

        private void Sub4_DropDownClosed(object sender, EventArgs e)
        {
            if (Sub4.Text == "" || Sub4.Text == "Vacio")
            {
                txtSub4.IsEnabled = false;
                Sub4.Items.Clear();
                Sub4.Items.Add("Seleccione");
                Sub4.SelectedIndex = 0;
            }
            else
            {
                txtSub4.IsEnabled = true;
            }

        }

        private void Sub5_DropDownOpened(object sender, EventArgs e)
        {
            int SelectIndex = Sub5.SelectedIndex;

            Sub5.Items.Clear();
            Sub5.Items.Add("Vacio");
            Sub5.Items.Add("HP");
            Sub5.Items.Add("ATK");
            Sub5.Items.Add("DEF");
            Sub5.Items.Add("VEL");
            Sub5.Items.Add("CRI. Rate");
            Sub5.Items.Add("CRI. Dmg");
            Sub5.Items.Add("RES");
            Sub5.Items.Add("ACC");
            Sub5.SelectedIndex = SelectIndex;
        }

        private void Sub5_DropDownClosed(object sender, EventArgs e)
        {
            if (Sub5.Text == "" || Sub5.Text == "Vacio")
            {

                txtSub5.IsEnabled = false;
                Sub5.Items.Clear();
                Sub5.Items.Add("Seleccione");
                Sub5.SelectedIndex = 0;
            }
            else
            {
                txtSub5.IsEnabled = true;
            }
        }


        //Evitar que el usuario inserte espacios en los stats del mob
        private void txtHP2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtATK2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtDEF2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtVEL2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtCR2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtCD2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtRES2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtACC2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtPrincipal_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtSub1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtSub2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtSub3_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtSub4_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void txtSub5_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        // Limitar los caracteres a numericos y/o "%"
        private void txtPrincipal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Principal.Text != "VEL")
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            }
            else
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            }
            base.OnPreviewTextInput(e);
        }

        private void txtSub1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Sub1.Text != "VEL")
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            }
            else
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            }
            base.OnPreviewTextInput(e);
        }

        private void txtSub2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Sub2.Text != "VEL")
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            }
            else
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            }
            base.OnPreviewTextInput(e);
        }

        private void txtSub3_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Sub3.Text != "VEL")
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            }
            else
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            }
            base.OnPreviewTextInput(e);
        }

        private void txtSub4_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Sub4.Text != "VEL")
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            }
            else
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            }
            base.OnPreviewTextInput(e);
        }

        private void txtSub5_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Sub5.Text != "VEL")
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            }
            else
            {
                e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            }
            base.OnPreviewTextInput(e);
        }

        private void txtHP2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            base.OnPreviewTextInput(e);
        }

        private void txtATK2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            base.OnPreviewTextInput(e);
        }

        private void txtDEF2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            base.OnPreviewTextInput(e);
        }

        private void txtVEL2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9]$");
            base.OnPreviewTextInput(e);
        }

        private void txtCR2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            base.OnPreviewTextInput(e);
        }

        private void txtCD2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            base.OnPreviewTextInput(e);
        }

        private void txtRES2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            base.OnPreviewTextInput(e);
        }

        private void txtACC2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text, @"[0-9%]$");
            base.OnPreviewTextInput(e);
        }



        //Limitar caracteres txtbox      
        private bool AreAllValidNumericChars(string str, string reg)
        {
            Regex _regex = new Regex(reg);
            foreach (char c in str)
            {

                if (!_regex.IsMatch(c.ToString()))
                {
                    return false;
                }
            }

            return true;
        }





        private void txtHP2_KeyUp(object sender, KeyEventArgs e)
        {
            Calculos();
        }

        private void txtDEF2_KeyUp(object sender, KeyEventArgs e)
        {
            Calculos();
        }

        private void txtCR2_KeyUp(object sender, KeyEventArgs e)
        {
            Calculos();
        }

        private void txtRES2_KeyUp(object sender, KeyEventArgs e)
        {
            Calculos();
        }

        private void txtATK2_KeyUp(object sender, KeyEventArgs e)
        {
            Calculos();
        }

        private void txtVEL2_KeyUp(object sender, KeyEventArgs e)
        {
            Calculos();
        }

        private void txtCD2_KeyUp(object sender, KeyEventArgs e)
        {
            Calculos();
        }

        private void txtACC2_KeyUp(object sender, KeyEventArgs e)
        {
            Calculos();
        }
    }


}

