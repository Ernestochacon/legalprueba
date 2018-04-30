
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
using Android.Provider;



namespace legalpro_cuestionarios
{
    class AdapterPregunta : BaseAdapter<lapregunta>
    {
        List<lapregunta> _lista;
        Activity _actividad;
        public string mivalor = "";
        

        public AdapterPregunta(Activity a, List<lapregunta> l) : base()
        {
            _actividad = a;
            _lista = l;
        }
        #region implemented abstract members of BaseAdapter
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            lapregunta item = _lista[position];
            View view = convertView;
            if (view == null)
                    view = _actividad.LayoutInflater.Inflate(Resource.Layout.lista2, null);


            view.FindViewById<TextView>(Resource.Id.titulo).Text = item.get_pregunta().variable;

          


            if (item.get_pregunta().preguntaCerrada != null)
            {
                
                view.FindViewById<TextView>(Resource.Id.resultado).Hint = item.get_pregunta().descripcion;
                if (item.get_pregunta().preguntaCerrada.respuestaOtros != null)
                {//se usa respuesta otros pues no se usa
                    int index = Int32.Parse(item.get_pregunta().preguntaCerrada.respuestaOtros) - 1;
                    view.FindViewById<TextView>(Resource.Id.resultado).Text = item.get_pregunta().preguntaCerrada.opciones[index].descripcion;
                }
                else {
                    view.FindViewById<TextView>(Resource.Id.resultado).Text = "";
                }
            }
            else {
                
                
                view.FindViewById<TextView>(Resource.Id.resultado).Hint = item.get_pregunta().descripcion;
               if (item.get_pregunta().preguntaAbierta.respuesta != null){
                    view.FindViewById<TextView>(Resource.Id.resultado).Text=item.get_pregunta().preguntaAbierta.respuesta;
                }
                



            }

            int id_img = _actividad.Resources.GetIdentifier("edit", "drawable", _actividad.PackageName);
            view.FindViewById<ImageView>(Resource.Id.logito).SetImageResource(id_img);

            return view;
        }

      
        public override int Count
        {
            get { return _lista.Count; }
        }
        #endregion

        #region implemented abstract members of BaseAdapter
        public override lapregunta this[int position]
        {
            get { return _lista[position]; }
        }
        #endregion

    }
}