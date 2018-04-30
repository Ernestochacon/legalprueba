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
    class Listageneral
    {
       public int formato;
       public  string titulo;
       public string descripcion;
        public int iconito;

        public Listageneral(int f, string t, string d,int i) {
            this.formato = f;
            this.titulo = t;
            this.descripcion = d;
            this.iconito = i;
        }

        void Set_formato(int f) {
            this.formato = f;
        }
        void Set_titulo(string t) {
            this.titulo = t;
        }
        void Set_descripcion(string d) {
            this.descripcion = d;
        }

        int Get_formato() {
            return this.formato;
        }
        string Get_titulo() {
            return this.titulo;
        }
        string Get_descripcion() {
            return this.descripcion;
        }
            }

}