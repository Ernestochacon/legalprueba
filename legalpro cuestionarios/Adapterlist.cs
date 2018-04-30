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
    class Adapterlist : BaseAdapter<Listageneral>
    {
        List<Listageneral> _lista;
        Activity _actividad;

        public Adapterlist(Activity a,List<Listageneral> l):base()
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
            Listageneral item = _lista[position];
            View view = convertView;
            if (view == null)
                view = _actividad.LayoutInflater.Inflate(Resource.Layout.lista1, null);
            
            
            view.FindViewById<TextView>(Resource.Id.descripcion).Text = "Formato: "+item.formato+" "+item.descripcion;
            view.FindViewById<TextView>(Resource.Id.titulo).Text = item.titulo;

            int id_img = 0;
            if (item.iconito == 1)
            {
                id_img = _actividad.Resources.GetIdentifier("edit", "drawable", _actividad.PackageName);
            }
            else if (item.iconito == 2) {
                id_img = _actividad.Resources.GetIdentifier("ok", "drawable", _actividad.PackageName);
            }
            else if (item.iconito==3) {
                id_img = _actividad.Resources.GetIdentifier("cancelar", "drawable", _actividad.PackageName);
            }

            

            //int id_img = _actividad.Resources.GetIdentifier("edit", "drawable", _actividad.PackageName);
            view.FindViewById<ImageView>(Resource.Id.logito).SetImageResource(id_img);
            
            return view;
        }

        public override int Count
        {
            get { return _lista.Count; }
        }
        #endregion

        #region implemented abstract members of BaseAdapter
        public override Listageneral this[int position]
        {
            get { return _lista[position]; }
        }
        #endregion

    }
}