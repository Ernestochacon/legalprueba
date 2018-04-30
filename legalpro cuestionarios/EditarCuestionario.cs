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
using Newtonsoft.Json;
using Android.Provider;
using System.Xml;
using Android.Graphics;
using Xamarin.Android;

namespace legalpro_cuestionarios
{
    [Activity(Label = "Editar cuestionario", WindowSoftInputMode = SoftInput.AdjustPan, ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class EditarCuestionario : Activity

    {
        public static string Legalproprefs = "legalpropref";
        ISharedPreferences opciones;
        bool click;
        ListView lista_modelos;
        sqlite_database_movements db;
        List<lapregunta> lista;
        List<lapregunta> lista_filtrada;
        Cuestionarioxml cuestionarioactual;
        int valor_de_spinner;
        ImageView imagen;
        string modelo;
        int id_formato;
        int id_modelo;
        Guardados g;
        bool anexos;
        ImageView imageviewfoto;
        Bitmap foto;
        int anexo_seleccionado;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Cuestionario);
            opciones = GetSharedPreferences(Legalproprefs, 0);
            if (opciones.GetString("nm_opr", "").Length == 0)
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(intent);
            }
            anexos = true;
            anexo_seleccionado = 0;
            db = new sqlite_database_movements();
            
            id_formato = Intent.GetIntExtra("id_formato", 0);
            lista = new List<lapregunta>();
            if (id_formato == 0)
            {
            FindViewById<ImageView>(Resource.Id.g_nube).Visibility = ViewStates.Gone;
                FindViewById<TextView>(Resource.Id.terminado).Visibility = ViewStates.Gone;
                modelo = Intent.GetStringExtra("modelo") ?? "";
            id_formato = Intent.GetIntExtra("id_formato", 0);
            id_modelo = Intent.GetIntExtra("id_modelo", 0);
            

                Cuestionarios actual = db.get_cuestionarioS(id_modelo, opciones.GetInt("instancia", 0), opciones.GetString("db", ""));

                imagen = FindViewById<ImageView>(Resource.Id.siguiente);
                imagen.Click += (s, e) =>
                {
                    if (anexos)
                    {
                        ListView listaaa = FindViewById<ListView>(Resource.Id.listaa);
                        listaaa.Visibility = ViewStates.Visible;
                        FindViewById<ListView>(Resource.Id.listam).Visibility = ViewStates.Gone;
                        anexos = false;
                    }
                    else {
                        ListView listaaa = FindViewById<ListView>(Resource.Id.listaa);
                        listaaa.Visibility = ViewStates.Gone;
                        FindViewById<ListView>(Resource.Id.listam).Visibility = ViewStates.Visible;
                        anexos = true;
                    }
                    

            };

                imagen = FindViewById<ImageView>(Resource.Id.guardar);
                imagen.Click += (s, e) =>
                {

                    Guardados guardar = new Guardados();
                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Guardar en local");
                    LayoutInflater inflater = LayoutInflater.From(this);
                    View dialogview = inflater.Inflate(Resource.Layout.lista2, null);
                    TextView titulo;
                    EditText editText = dialogview.FindViewById<EditText>(Resource.Id.resultado);
                    editText.Text = "";
                    editText.SetTextColor(Color.White);
                    editText.Focusable = true;
                    editText.FocusableInTouchMode = true;
                    dialogview.FindViewById<LinearLayout>(Resource.Id.templatt).SetBackgroundColor(Color.Transparent);
                    titulo = dialogview.FindViewById<TextView>(Resource.Id.titulo);
                    titulo.Text = "Nombre del tramite";
                    titulo.SetTextColor(Android.Graphics.Color.White);

                    builder.SetView(dialogview);

                    builder.SetPositiveButton("Aceptar", (sender2, args) =>
                    {
                        if (editText.Text.Length == 0)
                            guardar.nombre = "Nuevo  " + modelo;
                        else guardar.nombre = modelo + " " + editText.Text;
                        guardar.guardado = 0;
                        guardar.usuario = opciones.GetInt("id_opr", 0);
                        guardar.xml = JsonConvert.SerializeObject(cuestionarioactual);//se guarda el cuestionario
                        guardar.id_modelo = id_modelo;
                        guardar.nm_modelo = modelo;
                        guardar.container = "LegalPro Produccion/"+ opciones.GetString("nm_str", "")+"/"+ opciones.GetInt("instancia", 0)+"/";
                        guardar.containe2r = "LegalPro Produccion/";
                        guardar.id_instancia = opciones.GetInt("instancia", 0);

                        WebReference1.preguntaDTO[] preguntas = new WebReference1.preguntaDTO[lista.Count];
                        for (int y = 0; y < preguntas.Length; y++)
                        {
                            preguntas[y] = lista[y].get_pregunta();                        }

                        guardar.preguntas = JsonConvert.SerializeObject(preguntas);
                        db.insert_guardado(guardar, opciones.GetString("db", ""));
                        Intent intent = new Intent(this.ApplicationContext, typeof(Activity1));
                        StartActivity(intent);
                        Toast.MakeText(this.ApplicationContext, "Guardado local revisa en VER lISTA CUESTIONARIOS, para terminarlo", ToastLength.Long).Show();

                        click = true;
                    });
                    builder.SetNegativeButton("Cancelar", (sender2, args) =>
                    {
                        click = true;
                    });
                    builder.Show();

                };

                if (modelo.Length > 0)
                {
                    string json = opciones.GetString(modelo, "");
                    
                    WebReference1.preguntaDTO[] preguntas = JsonConvert.DeserializeObject<WebReference1.preguntaDTO[]>(json);
                    for (int y = 0; y < preguntas.Length; y++)
                    {
                        lapregunta temp = new lapregunta();
                        temp.pos_list = y;
                        temp.set_pregunta(preguntas[y]);
                        lista.Add(temp);
                    }
                    //lista = preguntas.OfType<WebReference1.preguntaDTO>().ToList();
                    cuestionarioactual = JsonConvert.DeserializeObject<Cuestionarioxml>(actual.xml);
                    //cuestionarioactual.los_anexos= JsonConvert.DeserializeObject<List<anexos>>(actual.anexos);
                    cuestionarioactual.variables = new Dictionary<string, string>();
                    for (int xx = 0; xx < preguntas.Length; xx++)
                    {

                        if (preguntas[xx].preguntaCerrada != null)
                            cuestionarioactual.variables[preguntas[xx].variable] = "1";
                        else
                            cuestionarioactual.variables[preguntas[xx].variable] = "";
                    }
                    //variables del cuestionario listas para modificarse
                    //fin del caso nuevo
                }
                
            }

            else {
                g= db.get_guardado(id_formato, opciones.GetInt("instancia", 0), opciones.GetString("db", ""));
                modelo = g.nm_modelo;                
                int id_modelo = g.id_modelo;
                cuestionarioactual = JsonConvert.DeserializeObject<Cuestionarioxml>(g.xml);
                //lista = JsonConvert.DeserializeObject <List<lapregunta>>(g.preguntas);

                WebReference1.preguntaDTO[] preguntas = JsonConvert.DeserializeObject<WebReference1.preguntaDTO[]>(g.preguntas);
                for (int y = 0; y < preguntas.Length; y++)
                {
                    lapregunta temp = new lapregunta();
                    temp.pos_list = y;
                    temp.set_pregunta(preguntas[y]);
                    lista.Add(temp);
                }

                
                imagen = FindViewById<ImageView>(Resource.Id.siguiente);
                imagen.Click += (s, e) =>
                {
                    if (anexos)
                    {
                        
                        ListView listaaa = FindViewById<ListView>(Resource.Id.listaa);//oculta

                        int cx = (listaaa.Left + listaaa.Right) / 2;
                        int cy = (listaaa.Top + listaaa.Bottom) / 2;

                        int finalRadius = Math.Max(listaaa.Width, listaaa.Height);

                        Android.Animation.Animator anim =
                                    ViewAnimationUtils.CreateCircularReveal(listaaa, cx, cy, 0, finalRadius);

                        listaaa.Visibility = ViewStates.Visible;
                        anim.Start();
                        FindViewById<ListView>(Resource.Id.listam).Visibility = ViewStates.Gone;

                        anexos = false;
                    }
                    else
                    {
                        
                        ListView listaaa = FindViewById<ListView>(Resource.Id.listam);//oculta
                        int cx = (listaaa.Left + listaaa.Right) / 2;
                        int cy = (listaaa.Top + listaaa.Bottom) / 2;

                        int finalRadius = Math.Max(listaaa.Width, listaaa.Height);

                        Android.Animation.Animator anim =
                                    ViewAnimationUtils.CreateCircularReveal(listaaa, cx, cy, 0, finalRadius);

                        listaaa.Visibility = ViewStates.Visible;
                        anim.Start();
                        FindViewById<ListView>(Resource.Id.listaa).Visibility = ViewStates.Gone; ;
                        anexos = true;
                    }
                };

                imagen = FindViewById<ImageView>(Resource.Id.guardar);
                imagen.Click += (s, e) =>
                {

                    //Guardados guardar = new Guardados();
                    var builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Guardar en local");
                    LayoutInflater inflater = LayoutInflater.From(this);
                    View dialogview = inflater.Inflate(Resource.Layout.lista2, null);
                    TextView titulo;
                    EditText editText = dialogview.FindViewById<EditText>(Resource.Id.resultado);
                    editText.Text = "";
                    editText.SetTextColor(Android.Graphics.Color.White);
                    editText.Focusable = false;
                    editText.FocusableInTouchMode = false;
                    dialogview.FindViewById<LinearLayout>(Resource.Id.templatt).SetBackgroundColor(Android.Graphics.Color.Transparent);
                    titulo = dialogview.FindViewById<TextView>(Resource.Id.titulo);
                    titulo.Text = "El archivo se sobreescribira";
                    titulo.SetTextColor(Android.Graphics.Color.White);

                    builder.SetView(dialogview);

                    builder.SetPositiveButton("Aceptar", (sender2, args) =>
                    {
                        
                        g.xml = JsonConvert.SerializeObject(cuestionarioactual);
                        WebReference1.preguntaDTO[] p = new WebReference1.preguntaDTO[lista.Count];
                        for (int y = 0; y < p.Length; y++)
                        {
                            p[y] = lista[y].get_pregunta();
                        }

                        g.preguntas = JsonConvert.SerializeObject(p);

                        if (db.update_guardado(g, opciones.GetString("db", ""))>0)
                            Toast.MakeText(this.ApplicationContext, "Guardado", ToastLength.Long).Show();
                        else
                            Toast.MakeText(this.ApplicationContext, "Error al guardar, intentelo otra vez", ToastLength.Long).Show();

                    });
                    builder.SetNegativeButton("Cancelar", (sender2, args) =>
                    {
                        click = true;
                    });
                    builder.Show();

                };

            }

            
            click = true;
            filtrar_lista();
            pintar_lista();
            var btnCamera = FindViewById<Button>(Resource.Id.buttonc);
            imageviewfoto = FindViewById<ImageView>(Resource.Id.imageViewfoto);

            btnCamera.Click += btnCamera_click;

            btnCamera= FindViewById<Button>(Resource.Id.buttona);
            btnCamera.Click += btnCamera_aceptar;

            btnCamera= FindViewById<Button>(Resource.Id.buttonx);
            btnCamera.Click += btnCamera_cancelar;

            FindViewById<ImageView>(Resource.Id.g_nube).Click += btnfinalizar;
            FindViewById<Button>(Resource.Id.buttonf).Click += btnfile_click;
            //btnCamera.Click += btnfinalizar;

            FindViewById<ImageView>(Resource.Id.imageView4).Click += (sender, e) =>
            {

                var builder = new AlertDialog.Builder(this);
                builder.SetTitle("Aviso");
                builder.SetMessage("Se cerrara la sesión ¿está seguro?");
                builder.SetPositiveButton("Aceptar", (sender2, args) => {


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
                builder.SetNegativeButton("Cancelar", (sender2, args) => {

                });
                builder.Show();

            };
        }

        void filtrar_lista() {
            for (int x = 0; x < cuestionarioactual.paginas.Length; x++)
            {//obtener preguntas por pagina
                if (cuestionarioactual.paginas[x].condicion != null)
                {
                    try {
                        if (!evaluacion_condicional(cuestionarioactual.paginas[x].condicion, cuestionarioactual.variables))
                            continue;
                    }
                    catch (Exception ee) {
                        continue;
                    }

                }
                if (cuestionarioactual.paginas[x].preguntaspagina == null) cuestionarioactual.paginas[x].preguntaspagina = new List<lapregunta>();
                else cuestionarioactual.paginas[x].preguntaspagina.Clear();
                for (int xx = 0; xx < cuestionarioactual.paginas[x].secciones.Length; xx++)
                {
                    if (cuestionarioactual.paginas[x].secciones[xx].condicion != null)
                    {
                        try
                        {
                            if (!evaluacion_condicional(cuestionarioactual.paginas[x].secciones[xx].condicion, cuestionarioactual.variables))
                                continue;
                        }
                        catch (Exception eee) {
                            continue;
                        }

                    }
                    for (int xxx = 0; xxx < cuestionarioactual.paginas[x].secciones[xx].parrafoo.Length; xxx++)
                    {
                        if (cuestionarioactual.paginas[x].secciones[xx].parrafoo[xxx].condicion != null)
                        {
                            try
                            {
                                if (!evaluacion_condicional(cuestionarioactual.paginas[x].secciones[xx].parrafoo[xxx].condicion, cuestionarioactual.variables))
                                    continue;
                            }
                            catch (Exception eee) {
                                continue;

                            }
                        }
                        for (int xxxx = 0; xxxx < cuestionarioactual.paginas[x].secciones[xx].parrafoo[xxx].preguntas.Length; xxxx++)
                        {
                            lapregunta preguntax;
                           
                                 preguntax= lista.Find(pregunt => pregunt.get_pregunta().variable == cuestionarioactual.paginas[x].secciones[xx].parrafoo[xxx].preguntas[xxxx]);

                            if (preguntax != null) {
                                if (preguntax.get_pregunta().condicion != null) {
                                    try {
                                        XmlElement condicionn = xmlcondicion(preguntax.get_pregunta().condicion);
                                        if (!evaluacion_condicional(condicionn, cuestionarioactual.variables))
                                        {
                                            continue;
                                        }
                                    }
                                    catch(Exception eeee){
                                        continue;
                                    }
                                }
                                cuestionarioactual.paginas[x].preguntaspagina.Add(preguntax);
                            }
                            

                        }
                    }
                }
            }
            lista_filtrada = new List<lapregunta>();
            for (int x = 0; x < cuestionarioactual.paginas.Length; x++)
            {
                if (cuestionarioactual.paginas[x].preguntaspagina != null)
                    lista_filtrada.AddRange(cuestionarioactual.paginas[x].preguntaspagina);
            }
        }

        public void pintar_lista() {

            ListView anexos_list = FindViewById<ListView>(Resource.Id.listaa);
            anexos_list.Adapter = new AdapterAnexos(this,cuestionarioactual.los_anexos);
            anexos_list.ChoiceMode = ChoiceMode.Single;
            anexos_list.ItemLongClick += OnAnexosItemClick;

            lista_modelos = FindViewById<ListView>(Resource.Id.listam);
            lista_modelos.Adapter = new AdapterPregunta(this, lista_filtrada);
            lista_modelos.ChoiceMode = ChoiceMode.Single;
            lista_modelos.ItemLongClick += OnListItemClick;

            
        }

        void clicksiguiente(object sender, AdapterView.IOnClickListener e) {

        }

        void OnListItemClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            /*
            var listView = sender as ListView;
            var t = lista_filtrada[e.Position];
            Toast.MakeText(this, t.get_pregunta().variable, ToastLength.Short).Show();*/
            if (click)
            {
                click = false;
                int index = e.Position;
                var builder = new AlertDialog.Builder(this);
                builder.SetTitle(lista_filtrada[index].get_pregunta().descripcion);
                LayoutInflater inflater = LayoutInflater.From(this);
                View dialogview=inflater.Inflate(Resource.Layout.lista2, null);
                TextView titulo;
                if (lista_filtrada[index].get_pregunta().preguntaCerrada != null)
                {
                    dialogview = inflater.Inflate(Resource.Layout.Lista3, null);
                    Spinner spinner = dialogview.FindViewById<Spinner>(Resource.Id.opciones);
                    List<string> opciones = new List<string>();
                    for (int x = 0; x < lista_filtrada[index].get_pregunta().preguntaCerrada.opciones.Count(); x++)
                    {
                        opciones.Add(lista_filtrada[index].get_pregunta().preguntaCerrada.opciones[x].descripcion);
                    }
                    var adapter = new ArrayAdapter<string>(dialogview.Context, Resource.Layout.spinner_kayout, opciones);
                    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinner.Adapter = adapter;

                    if (lista_filtrada[index].get_pregunta().preguntaCerrada.respuestaOtros != null)
                    {//se usa respuesta otros pues no se usa
                        spinner.SetSelection(Int32.Parse(lista_filtrada[index].get_pregunta().preguntaCerrada.respuestaOtros)-1);
                    }

                    spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
                }
                else {
                     
                    EditText editText = dialogview.FindViewById<EditText>(Resource.Id.resultado);
                    editText.Text = lista_filtrada[index].get_pregunta().preguntaAbierta.respuesta;
                    editText.SetTextColor(Android.Graphics.Color.White);
                    editText.Focusable = true;
                    editText.FocusableInTouchMode = true;
                    dialogview.FindViewById<LinearLayout>(Resource.Id.templatt).SetBackgroundColor(Android.Graphics.Color.Transparent);
                }
                titulo = dialogview.FindViewById<TextView>(Resource.Id.titulo);
                titulo.Text = lista_filtrada[index].get_pregunta().variable;
                titulo.SetTextColor(Android.Graphics.Color.White);

                builder.SetView(dialogview);

                
                builder.SetPositiveButton("Aceptar", (sender2, args) => {
                    if (lista_filtrada[index].get_pregunta().preguntaCerrada != null)
                    {
                        cuestionarioactual.variables[lista_filtrada[index].get_pregunta().variable] = "" + valor_de_spinner;
                        lista[lista_filtrada[index].pos_list].get_pregunta().preguntaCerrada.respuestaOtros = "" + valor_de_spinner;
                        filtrar_lista();
                        pintar_lista();
                    }
                    else {
                        cuestionarioactual.variables[lista_filtrada[index].get_pregunta().variable] = dialogview.FindViewById<EditText>(Resource.Id.resultado).Text;
                        lista_filtrada[index].get_pregunta().preguntaAbierta.respuesta= dialogview.FindViewById<EditText>(Resource.Id.resultado).Text;
                        pintar_lista();
                    }
                    click = true;  });
                builder.SetNegativeButton("Cancelar", (sender2, args) => {
                    click = true;
                });
                builder.Show();
            }
        }

        void OnAnexosItemClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            anexo_seleccionado = e.Position;
            FindViewById<LinearLayout>(Resource.Id.fotolayout).Visibility= ViewStates.Visible;
            Bitmap bitmap=null;
                if (cuestionarioactual.los_anexos[e.Position].archivo!=null) {
                bitmap = BitmapFactory.DecodeByteArray(cuestionarioactual.los_anexos[e.Position].archivo, 0, cuestionarioactual.los_anexos[e.Position].archivo.Length);
            }
               
            imageviewfoto.SetImageBitmap(bitmap);
        }

        bool evaluacion_condicional(XmlElement condicion, Dictionary<string, string> vatiables)
        {
            XmlNodeList comparative;
            XmlNodeList comparative2;
            string comparator;
            string nodo;
            bool respuesta = false;

            comparative = condicion.GetElementsByTagName("comparative");
            comparative2 = condicion.GetElementsByTagName("comparative2");
            nodo = ((XmlElement)condicion.GetElementsByTagName("comparative")[0]).GetElementsByTagName("variable")[0].InnerText;
            comparator = vatiables[nodo];
            //(XmlElement)xmlvariables.GetElementsByTagName(nodo)[0];

            if (comparator.Equals(((XmlElement)condicion.GetElementsByTagName("comparative")[0]).GetElementsByTagName("numeroOpcion")[0].InnerText))
            {
                respuesta = true;
            }
            for (int x = 0; x < comparative2.Count; x++)
            {
                if (((XmlElement)comparative2[x]).GetElementsByTagName("opLogico")[0].InnerText.Equals("OR"))
                {
                    nodo = ((XmlElement)((XmlElement)comparative2[x]).GetElementsByTagName("variable")[0]).InnerText;
                    comparator = vatiables[nodo];
                    //(XmlElement)xmlvariables.GetElementsByTagName(nodo)[0];
                    if (comparator.Equals((((XmlElement)comparative2[x]).GetElementsByTagName("numeroOpcion")[0].InnerText)))
                    {
                        respuesta = true;
                    }

                }
                if (((XmlElement)comparative2[x]).GetElementsByTagName("opLogico")[0].InnerText.Equals("AND"))
                {
                    nodo = ((XmlElement)((XmlElement)comparative2[x]).GetElementsByTagName("variable")[0]).InnerText;
                    comparator = vatiables[nodo];
                    //(XmlElement)xmlvariables.GetElementsByTagName(nodo)[0];
                    if (!comparator.Equals((((XmlElement)comparative2[x]).GetElementsByTagName("numeroOpcion")[0].InnerText)))
                    {
                        respuesta = false;
                        return respuesta;
                    }
                    else { respuesta = true; }
                }
            }


            return respuesta;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            valor_de_spinner = e.Position + 1;
        }

        public static XmlElement  xmlcondicion(object o)
        {
            XmlDocument doc = new XmlDocument();

            using (XmlWriter writer = doc.CreateNavigator().AppendChild())
            {
                new System.Xml.Serialization.XmlSerializer(o.GetType()).Serialize(writer, o);
            }

            return doc.DocumentElement;
        }

        private void btnCamera_click(object sender, EventArgs e) {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, 0);
        }

        private void btnfile_click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionGetContent);
            intent.SetType("*/*");
            //String[] mimetypes = { "image/*", "video/*" };
            //intent.PutExtra(Intent.ExtraMimeTypes, mimetypes);
            StartActivityForResult(Intent.CreateChooser(intent,"Selecciona"),0);
        }

        private void btnfinalizar(object sender, EventArgs e) {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Guardar en local");
            LayoutInflater inflater = LayoutInflater.From(this);
            View dialogview = inflater.Inflate(Resource.Layout.lista2, null);
            TextView titulo;
            EditText editText = dialogview.FindViewById<EditText>(Resource.Id.resultado);
            editText.Text = "";
            editText.SetTextColor(Android.Graphics.Color.White);
            editText.Focusable = false;
            editText.FocusableInTouchMode = false;
            dialogview.FindViewById<LinearLayout>(Resource.Id.templatt).SetBackgroundColor(Android.Graphics.Color.Transparent);
            titulo = dialogview.FindViewById<TextView>(Resource.Id.titulo);
            titulo.Text = "El cuestionario se subirá a la red, si no estás conectado a internet se subirá cuando halla conexión.";
            titulo.SetTextColor(Android.Graphics.Color.White);

            builder.SetView(dialogview);

            builder.SetPositiveButton("Aceptar", (sender2, args) =>
            {

                g.xml = JsonConvert.SerializeObject(cuestionarioactual);
                WebReference1.preguntaDTO[] p = new WebReference1.preguntaDTO[lista.Count];
                for (int y = 0; y < p.Length; y++)
                {
                    p[y] = lista[y].get_pregunta();
                }

                g.preguntas = JsonConvert.SerializeObject(p);
                g.guardado = 1;

                if (db.update_guardado(g, opciones.GetString("db", "")) > 0)
                    Toast.MakeText(this.ApplicationContext, "", ToastLength.Long).Show();
                else
                    Toast.MakeText(this.ApplicationContext, "Error al guardar, intentelo otra vez", ToastLength.Long).Show();

            });
            builder.SetNegativeButton("Cancelar", (sender2, args) =>
            {
                click = true;
            });
            builder.Show();
        }

        private void btnCamera_cancelar(object sender, EventArgs e)
        {
            FindViewById<LinearLayout>(Resource.Id.fotolayout).Visibility = ViewStates.Gone;
            foto = null;
        }

        private void btnCamera_aceptar(object sender, EventArgs e)
        {
            if (foto == null)
            {
                Toast.MakeText(this, "No se a tomado foto", ToastLength.Short).Show();
                //foto = createImage(100, 100, 1, 2, 3, 4);
            }
            else {
                byte[] bitmapData;
               
                using (var stream = new System.IO.MemoryStream())
                {
                    foto.Compress(Bitmap.CompressFormat.Png, 0, stream);
                    bitmapData = stream.ToArray();
                }
                cuestionarioactual.los_anexos[anexo_seleccionado].archivo = bitmapData;
                foto = null;
                FindViewById<LinearLayout>(Resource.Id.fotolayout).Visibility = ViewStates.Gone;
                pintar_lista();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode==Result.Ok) {
                try
                {
                    //string path = GetPathToImage(data.Data);

                    foto = (Bitmap)data.Extras.Get("data");
                    imageviewfoto.SetImageBitmap(foto);
                }
                catch (Exception a)
                {
                    string path = GetRealPathFromURI(this, data.Data);
                    string name= path.Substring(path.LastIndexOf("/", StringComparison.Ordinal) + 1);

                    cuestionarioactual.los_anexos[anexo_seleccionado].archivo = System.IO.File.ReadAllBytes(path);
                    cuestionarioactual.los_anexos[anexo_seleccionado].nm_archivo = name;
                    foto = null;
                    FindViewById<LinearLayout>(Resource.Id.fotolayout).Visibility = ViewStates.Gone;
                    pintar_lista();
                }
            }
            
                    
        }

        public static Bitmap createImage(int width, int height, int color1,int color2, int color3, int color4 )
        {
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            Paint paint = new Paint();
            paint.SetARGB(color1,color2,color3,color4);
            canvas.DrawRect(0F, 0F, (float)width, (float)height, paint);
            return bitmap;
        }
       

        public static string GetRealPathFromURI(Context context, Android.Net.Uri uri)
        {

            bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

            // DocumentProvider
            if (isKitKat && Android.Provider.DocumentsContract.IsDocumentUri(context, uri))
            {
                // ExternalStorageProvider
                if (isExternalStorageDocument(uri))
                {
                    string docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }

                    // TODO handle non-primary volumes
                }
                // DownloadsProvider
                else if (isDownloadsDocument(uri))
                {

                    string id = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(Android.Net.Uri.Parse("content://downloads/public_downloads"), Convert.ToInt64(id));

                    return getDataColumn(context, contentUri, null, null);
                }
                // MediaProvider
                else if (isMediaDocument(uri))
                {
                    string docId = Android.Provider.DocumentsContract.GetDocumentId(uri);
                    string[] split = docId.Split(':');
                    string type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = Android.Provider.MediaStore.Audio.Media.ExternalContentUri;
                    }

                    string selection = "_id=?";
                    string[] selectionArgs = new string[] {
                    split[1]
            };

                    return getDataColumn(context, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return getDataColumn(context, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }
        
        public static String getDataColumn(Context context, Android.Net.Uri uri, String selection,
                String[] selectionArgs)
        {

            Android.Database.ICursor cursor = null;
            string column = "_data";
            string[] projection = {
                column
            };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs,
                        null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int column_index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(column_index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }


        
        public static bool isExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

       
        public static bool isDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

      
        public static bool isMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

    }



    public class Cuestionarioxml {
        public paginaxml[] paginas;
        public List<anexos> los_anexos;
        public Dictionary<string, string> variables;
        
    }
    public class paginaxml {
        public List<string> variablesc;
        public XmlElement condicion;
        public int numero;
        public List<lapregunta> preguntaspagina;
        public seccionxml[] secciones;

        public paginaxml()
        {
            this.variablesc = new List<string>();
        }
    }
    public class seccionxml {
       public List<string> variablesc;
        public XmlElement condicion;
       public  parrafoxml[] parrafoo;

        public seccionxml()
        {
            this.variablesc = new List<string>();
        }
    }

    public class parrafoxml {
        public XmlElement condicion;
       public  string[] preguntas;
    }

    public class lapregunta {
        private WebReference1.preguntaDTO pregunta;
        public int pos_list;

        public void set_pregunta(WebReference1.preguntaDTO p) {
            pregunta = p;
        }

        public WebReference1.preguntaDTO get_pregunta() {
            return pregunta;
        }

        
    }

    public class anexos {
        public XmlElement condicion;
        public string descrAnexo;
        public string iDAnexo;
        public string anexo_url;
        public string nm_archivo;
        public byte[] archivo;
        //public Bitmap archivo;
    }
}