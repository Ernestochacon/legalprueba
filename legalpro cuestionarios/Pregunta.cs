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
using legalpro_cuestionarios.WebReference1;

namespace legalpro_cuestionarios
{
    class Pregunta
    {
        WebReference1.preguntaDTO a;
        

        public Pregunta(preguntaDTO a)
        {
            this.a = a;
           
        }
    }
}