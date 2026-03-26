using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace ProyectoUWPenBlanco
{
    public sealed partial class MainPage : Page
    {
        private iPokemon pokemon;

        public MainPage()
        {
            this.InitializeComponent();
            pokemon = miPokemon;
        }

        private void cambiarVida(object sender, RangeBaseValueChangedEventArgs e)
        {
            pokemon.Vida = e.NewValue;
        }

        private void cambiarEnergía(object sender, RangeBaseValueChangedEventArgs e)
        {
            pokemon.Energia = e.NewValue;
        }

        private void EjecutarAtaqueFuerte(object sender, RoutedEventArgs e)
        {
            pokemon.animacionAtaqueFuerte();
        }

        private void EjecutarAtaqueFlojo(object sender, RoutedEventArgs e)
        {
            pokemon.animacionAtaqueFlojo();
        }

        private void EjecutarDefensa(object sender, RoutedEventArgs e)
        {
            pokemon.animacionDefensa();
        }

        private void EjecutarDescansar(object sender, RoutedEventArgs e)
        {
            pokemon.animacionDescasar();
        }

        private void activarIddle(object sender, RoutedEventArgs e)
        {
            pokemon.activarAniIdle(true);
        }

        private void desactivarIddle(object sender, RoutedEventArgs e)
        {
            pokemon.activarAniIdle(false);
        }

        private void desactivarCansado(object sender, RoutedEventArgs e)
        {
            pokemon.animacionNoCansado();
        }

        private void activarCansado(object sender, RoutedEventArgs e)
        {
            pokemon.animacionCansado();
        }

        private void activarHerido(object sender, RoutedEventArgs e)
        {
            pokemon.animacionHerido();
        }

        private void desactivarHerido(object sender, RoutedEventArgs e)
        {
            pokemon.animacionNoHerido();
        }

        private void desactivarEscudo(object sender, RoutedEventArgs e)
        {
            pokemon.verEscudo(false);
        }

        private void activarEscudo(object sender, RoutedEventArgs e)
        {
            pokemon.verEscudo(true);
        }

        private void verFondo(object sender, RoutedEventArgs e)
        {
            pokemon.verFondo(true);
        }

        private void noVerFondo(object sender, RoutedEventArgs e)
        {
            pokemon.verFondo(false);
        }

        private void noVerFilaVida(object sender, RoutedEventArgs e)
        {
            pokemon.verFilaVida(false);
        }

        private void verFilaVida(object sender, RoutedEventArgs e)
        {
            pokemon.verFilaVida(true);
        }

        private void verFilaEnergia(object sender, RoutedEventArgs e)
        {
            pokemon.verFilaEnergia(true);
        }

        private void noVerFilaEnergía(object sender, RoutedEventArgs e)
        {
            pokemon.verFilaEnergia(false);
        }

        private void verPocimaVida(object sender, RoutedEventArgs e)
        {
            pokemon.verPocionVida(true);
        }

        private void noVerPocimaVida(object sender, RoutedEventArgs e)
        {
            pokemon.verPocionVida(false);
        }

        private void noVerPocimaEnergia(object sender, RoutedEventArgs e)
        {
            pokemon.verPocionEnergia(false);
        }

        private void verPocimaEnergia(object sender, RoutedEventArgs e)
        {
            pokemon.verPocionEnergia(true);
        }

        private void verNombrePokemon(object sender, RoutedEventArgs e)
        {
            pokemon.verNombre(true);
        }

        private void noVerNombrePokemon(object sender, RoutedEventArgs e)
        {
            pokemon.verNombre(false);
        }

        private void cambiarTamano(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (miPokemon != null)
            {
                var escala = new CompositeTransform();
                escala.ScaleX = this.slTamano.Value / 100;
                escala.ScaleY = this.slTamano.Value / 100;
                miPokemon.RenderTransform = escala;
            }
        }
    }
}