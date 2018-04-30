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

namespace legalpro_cuestionarios
{
    class AdapterAnexos : BaseAdapter<anexos>
    {
        List<anexos> _lista;
        Activity _actividad;

        public AdapterAnexos(Activity a, List<anexos> l) : base()
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
            anexos item = _lista[position];
            View view = convertView;
            if (view == null)
                view = _actividad.LayoutInflater.Inflate(Resource.Layout.lista1, null);


            view.FindViewById<TextView>(Resource.Id.descripcion).Text = item.descrAnexo;
            view.FindViewById<TextView>(Resource.Id.titulo).Text = item.iDAnexo;
            if(item.archivo==null)
            view.FindViewById<TextView>(Resource.Id.titulo).SetTextColor(Android.Graphics.Color.Red);
            else
                view.FindViewById<TextView>(Resource.Id.titulo).SetTextColor(Android.Graphics.Color.Green);
            int id_img = 0;
            
            
                id_img = _actividad.Resources.GetIdentifier("edit", "drawable", _actividad.PackageName);
            



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
        public override anexos this[int position]
        {
            get { return _lista[position]; }
        }
        #endregion

    }
}