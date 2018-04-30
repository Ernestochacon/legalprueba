using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using Newtonsoft.Json;
using System.Xml;


namespace legalpro_cuestionarios
{
    [Activity(Label = "Modelos disponibles", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class Showmodelos : Activity
    {
        List<Listageneral> general = new List<Listageneral>();
        public static string Legalproprefs = "legalpropref";
        ISharedPreferences opciones;
        ListView lista_modelos;
        double tiempo;
        Boolean click;
        string error;
        List<int> locales;
        sqlite_database_movements db;

        string anexosxml;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Lista);
            // Create your application hereru
            click = true;
            anexosxml = "";
            opciones = GetSharedPreferences(Legalproprefs, 0);
            if (opciones.GetString("nm_opr", "").Length == 0)
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(intent);
            }

            RunOnUiThread(() => {
                System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                timer.Start();
                Get_modelos();
                RunOnUiThread(() => List_the_list());
                timer.Stop();
                tiempo = timer.Elapsed.TotalSeconds;
            });

            FindViewById<ImageView>(Resource.Id.imageView4).Click += (sender, e) =>
            {

                var builder2 = new AlertDialog.Builder(this);
                builder2.SetTitle("Aviso");
                builder2.SetMessage("Se cerrara la sesión ¿está seguro?");
                builder2.SetPositiveButton("Aceptar", (sender2, args) => {


                    Toast.MakeText(this.ApplicationContext, "Adios", ToastLength.Long).Show();
                    //resultado.Text = respuesta.INSTANCIA.ID_INSTANCIA + " Operador: "+respuesta.id_Opr+" "+respuesta.nm_Oper;
                    ISharedPreferencesEditor editor = opciones.Edit();
                    editor.PutString("nm_opr", "");
                    editor.PutInt("id_opr", 0);
                    editor.PutString("nm_str", "");
                    editor.PutInt("instancia", 0);
                    editor.Commit();
                    StartActivity(typeof(MainActivity));

                });
                builder2.SetNegativeButton("Cancelar", (sender2, args) => {

                });
                builder2.Show();

            };

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Aviso");
            builder.SetMessage("Los Cuestionarios verdes ya estan descargados");
            builder.SetPositiveButton("OK", (sender2, args) => { });           
            builder.Show();



        }

        void Get_modelos()
        {
            db = new sqlite_database_movements();
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {

                WebReference1.EJBWebServicev2_0 service = new WebReference1.EJBWebServicev2_0();
                //WebReference1.getPreguntasResponse a = new WebReference1.getPreguntasResponse();           
                WebReference1.getModelo indata = new WebReference1.getModelo();
                indata.ID_Instancia = opciones.GetInt("instancia", 0);
                indata.oper = "TYP";
                indata.ID_MODELO = 0;
                indata.TY_MODELO = "F";
                indata.NOMBRE = "";

                try
                {
                    WebReference1.getModeloResponse respuesta = service.GET_ModeloW(indata);
                    if (respuesta.modelos == null)
                    {
                        //Toast.MakeText(this.ApplicationContext, "Error: Lista de cuestionarios nula", ToastLength.Long).Show();
                        error = "Error: Lista de cuestionarios nula";
                        return;
                    }
                    if (respuesta.modelos.Count() == 0)
                    {
                        //Toast.MakeText(this.ApplicationContext, "Error: No hay Cuestionarios para mostrar", ToastLength.Long).Show();
                        error = "Error: No hay Cuestionarios para mostrar";
                        return;
                    }
                    Listageneral temp;
                    //Toast.MakeText(this.ApplicationContext, respuesta.modelos.Count()+" Cuestionarios encontrados.", ToastLength.Long).Show();
                    error = respuesta.modelos.Count() + " Cuestionarios encontrados.";
                    locales = db.listam(opciones.GetString("db", ""), opciones.GetInt("instancia", 0));
                    for (int x = 0; x < respuesta.modelos.Count(); x++)
                    {
                        if (locales.Contains(respuesta.modelos[x].id_modelo)) temp = new Listageneral(respuesta.modelos[x].id_modelo, respuesta.modelos[x].nombre_cuestionario, "°", 2);//diferentes tipos de reporte 1 edit, 2 ok, 3 cancel
                        else temp = new Listageneral(respuesta.modelos[x].id_modelo, respuesta.modelos[x].nombre_cuestionario, "°", 3);//diferentes tipos de reporte 1 edit, 2 ok, 3 cancel
                        general.Add(temp);
                    }
                }
                catch (Exception aa)
                {
                    //Toast.MakeText(this.ApplicationContext, "Error: " + aa.Message, ToastLength.Long).Show();
                    error = "Error: " + aa.Message;
                    locales = db.listam(opciones.GetString("db", ""), opciones.GetInt("instancia", 0));
                    general = db.cuestionarioslocal(opciones.GetString("db",""),opciones.GetInt("instancia",0));
                }
            }
            else {
                Toast.MakeText(this.ApplicationContext, "Error: No hay coneccion a internet", ToastLength.Long).Show();
                error = "Error: No hay coneccion a internet";
                locales = db.listam(opciones.GetString("db", ""), opciones.GetInt("instancia", 0));
                general = db.cuestionarioslocal(opciones.GetString("db", ""), opciones.GetInt("instancia", 0));
            }
            

        }

        String get_xml(int modelo) {
            WebReference1.EJBWebServicev2_0 service = new WebReference1.EJBWebServicev2_0();
            WebReference1.getModeloResponse a = new WebReference1.getModeloResponse();           
            WebReference1.getModelo indata = new WebReference1.getModelo();
            indata.ID_Instancia= opciones.GetInt("instancia", 0);
            indata.ID_MODELO = modelo;
            indata.NOMBRE = "";
            indata.oper = "SNG";
            indata.TY_MODELO = "F";

            try
            {
                a = service.GET_ModeloW(indata);
                if (a.modelo == null)
                {
                    return "";
                }
                return a.modelo.xml;
            }
            catch (Exception aa)
            {
                //Toast.MakeText(this.ApplicationContext, "Error: " + aa.Message, ToastLength.Long).Show();
                Console.WriteLine(aa.StackTrace);
                return "";
            }
        }

        WebReference1.preguntaDTO[] get_preguntas(int modelo,string nombre) {
            WebReference1.EJBWebServicev2_0 service = new WebReference1.EJBWebServicev2_0();
            WebReference1.getPreguntasResponse respuesta = new WebReference1.getPreguntasResponse();
            WebReference1.getModelo indata = new WebReference1.getModelo();

            indata.ID_Instancia = opciones.GetInt("instancia", 0);
            indata.ID_MODELO = modelo;
            indata.NOMBRE = "";
            indata.oper = "SNG";
            indata.TY_MODELO = "F";

            try
            {

                respuesta = service.GET_PregunasW(indata);
                if (respuesta.preguntas.Count() > 0)
                {
                    ISharedPreferencesEditor editor = opciones.Edit();
                    string lista = JsonConvert.SerializeObject(respuesta.preguntas);
                    editor.PutString(nombre, lista);
                    editor.Commit();
                    return respuesta.preguntas;
                }
                else { return null; }
            }
            catch (Exception e) {
                return null;
            }
        }

        void List_the_list() {
            /*
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Aviso");
            builder.SetMessage("Los Cuestionarios verdes ya estan descargados");
            builder.SetPositiveButton("ok", (sender2, args) => { });
            builder.Show();*/

            lista_modelos = FindViewById<ListView>(Resource.Id.listam);
            lista_modelos.Adapter = new Adapterlist(this, general);
            Toast.MakeText(this.ApplicationContext, error, ToastLength.Long).Show();
            lista_modelos.ItemLongClick += new EventHandler<AdapterView.ItemLongClickEventArgs>(ItemLong_OnClick);
        }

        void Set_image(int resourse,int index) {
            //var a = lista_modelos.GetItemAtPosition(index);
            //var a = lista_modelos.SelectedItem;
            //int id_img = this.Resources.GetIdentifier(resourse, "drawable", this.PackageName);           
            //a.FindViewById<ImageView>(Resource.Id.logito).SetImageResource(id_img);
            general[index].iconito = resourse;
            List_the_list();
        }

        private void ItemLong_OnClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            if (click) {
                click = false;
                int index = e.Position;
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Aviso");
                builder.SetMessage(general[index].formato + " " + general[index].titulo);
                builder.SetPositiveButton("Descargar/Actualizar", (sender2, args) => { click = true; descargar(index);  });
                builder.SetNegativeButton("Crear Nuevo", (sender2, args) => {
                    click = true; Intent intent = new Intent(this.ApplicationContext, typeof(EditarCuestionario));
                    intent.PutExtra("modelo", general[index].titulo);
                    intent.PutExtra("id_modelo", general[index].formato);
                    intent.PutExtra("id_formato",0);
                StartActivity(intent);
                });
                builder.Show();
            }
           

            
        }

        public void descargar(int index) {
            if (click) Toast.MakeText(this.ApplicationContext, "Descargando Modelo:" + general[index].formato + " " + general[index].titulo, ToastLength.Long).Show();
            else Toast.MakeText(this.ApplicationContext, "Ya se esta descargando un modelo, espere un momento", ToastLength.Long).Show();

            if (click)
            {
                //new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                RunOnUiThread(() => {
                    click = false;
                    //var a = lista_modelos.GetChildAt(index);
                    // a.FindViewById<ProgressBar>(Resource.Id.cargando).Visibility=ViewStates.Visible;
                    int numero = lista_modelos.Count;
                    Cuestionarios cus = new Cuestionarios();
                    cus.id_cuestionario = general[index].formato;
                    cus.instancia = opciones.GetInt("instancia", 0);
                    cus.nombre = general[index].titulo;
                    cus.xml = get_xml(general[index].formato);
                    if (cus.xml.Length > 0)
                    {
                        string xmlcuestionarios = Xmls_a_objetos(cus.xml);
                        cus.xml = xmlcuestionarios;
                        //cus.anexos = anexosxml;
                        var db = new SQLiteAsyncConnection(opciones.GetString("db", ""));
                        var x = db.InsertAsync(cus);
                        if (!x.IsCompleted) db.UpdateAsync(cus);
                        Set_image(2, index);
                        get_preguntas(general[index].formato, general[index].titulo);
                    }
                    else if (locales.Contains(general[index].formato))
                    {
                        Set_image(2, index);
                    }
                    else
                    {
                        Set_image(3, index);
                    }
                    click = true;
                    //a = lista_modelos.GetChildAt(index);
                    //a.FindViewById<ProgressBar>(Resource.Id.cargando).Visibility = ViewStates.Gone;
                });
                //})).Start();
            }
        }

        public string Xmls_a_objetos(string xmls)
        {
            String json = "";
            XmlDocument reader = new XmlDocument();
            reader.LoadXml(xmls);
            XmlNodeList paginas;
            XmlNodeList secciones;
            XmlNodeList parrafos;
            XmlNodeList anexos;
            Cuestionarioxml cuestxml = new Cuestionarioxml();
            paginas = reader.GetElementsByTagName("pagina");
            anexos = reader.GetElementsByTagName("anexo");
            cuestxml.paginas = new paginaxml[paginas.Count];
            cuestxml.los_anexos = new List<anexos>();
           /* cuestxml.los_anexos = new anexos[anexos.Count];
            for (int a = 0; a < anexos.Count; a++) {
                cuestxml.los_anexos[a] = new anexos();
                cuestxml.los_anexos[a].condicion = (XmlElement)((XmlElement)anexos[a]).GetElementsByTagName("condicion")[0];
                cuestxml.los_anexos[a].descrAnexo = ((XmlElement)anexos[a]).GetAttribute("descrAnexo");
                cuestxml.los_anexos[a].iDAnexo = ((XmlElement)anexos[a]).GetAttribute("iDAnexo");
            }*/
            
            for (int pag=0;pag<paginas.Count;pag++) {
                paginaxml paginatemp = new paginaxml();
                

                secciones = ((XmlElement)paginas[pag]).GetElementsByTagName("seccion");
                paginatemp.secciones = new seccionxml[secciones.Count];
                if (((XmlElement)paginas[pag]).FirstChild.Name == "condicion") {

                    paginatemp.variablesc.AddRange(
                        variables_en_condicional(((XmlElement)secciones[0]))//mete todos los resultados
                        );
                    paginatemp.condicion = (((XmlElement)((XmlElement)paginas[pag]).FirstChild));
                }
                paginatemp.numero = Int32.Parse(((XmlElement)paginas[pag]).GetAttribute("numero"));

                for (int secc = 0; secc < secciones.Count; secc++) {//for de seccion
                    seccionxml secciontemporal = new seccionxml();
                    parrafos = ((XmlElement)secciones[secc]).GetElementsByTagName("parrafo");
                    secciontemporal.parrafoo = new parrafoxml[parrafos.Count];
                    if (((XmlElement)secciones[secc]).FirstChild.Name == "condicion")
                    {
                        secciontemporal.condicion = ((XmlElement)((XmlElement)secciones[secc]).FirstChild);
                        //meter lacondicion                  
                    }
                    for (int parr = 0; parr < parrafos.Count; parr++) {
                        parrafoxml parrafotemp = new parrafoxml();
                        XmlNodeList preguntas = ((XmlElement)parrafos[parr]).GetElementsByTagName("pregunta");
                        parrafotemp.preguntas = new string[preguntas.Count];
                        if (((XmlElement)parrafos[parr]).FirstChild.Name=="condicion") {
                            parrafotemp.condicion = ((XmlElement)((XmlElement)parrafos[parr]).FirstChild);
                        }
                        for (int pre = 0; pre < preguntas.Count; pre++)
                            parrafotemp.preguntas[pre] = ((XmlElement)preguntas[pre]).GetAttribute("variable");
                        secciontemporal.parrafoo[parr] = parrafotemp;
                    }
                    anexos=((XmlElement)secciones[secc]).GetElementsByTagName("anexos");
                    for (int a = 0; a < anexos.Count; a++)
                    {
                        XmlNodeList anexo = ((XmlElement)secciones[secc]).GetElementsByTagName("anexo");
                        for (int aa = 0; aa < anexo.Count; aa++) {
                            anexos tempu = new anexos();
                            tempu.condicion = (XmlElement)((XmlElement)anexo[aa]).GetElementsByTagName("condicion")[0];
                            tempu.descrAnexo = ((XmlElement)anexo[aa]).GetAttribute("descrAnexo");
                            tempu.iDAnexo = ((XmlElement)anexo[aa]).GetAttribute("iDAnexo");
                            tempu.anexo_url = "#{motorHTML.cuestionario.pagina[" + pag + "].seccion[" + secc + "].anexos["+a+"].anexo["+aa+"].archivo}";
                            cuestxml.los_anexos.Add(tempu);
                        }
                    }
                    paginatemp.secciones[secc] = secciontemporal;  
                }//for de secciones
                cuestxml.paginas[pag] = paginatemp;
            }//for de paginas
            //anexosxml = JsonConvert.SerializeObject(cuestxml.los_anexos);
            //cuestxml.los_anexos = null;
            json = JsonConvert.SerializeObject(cuestxml);
            
            return json;
        }

        List<string> variables_en_condicional(XmlElement condicion) {
            List<string> var = new List<string>();
            XmlNodeList variablescondionales = condicion.GetElementsByTagName("variable");
            for (int x = 0; x < variablescondionales.Count; x++)
                var.Add(variablescondionales[x].InnerText);
            return var;
        }

        

    }

}