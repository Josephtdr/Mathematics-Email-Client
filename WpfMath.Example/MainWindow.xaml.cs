﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfMath.Example
{
    public partial class MainWindow : Window
    {
        private TexFormulaParser formulaParser;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void renderButton_Click(object sender, RoutedEventArgs e)
        {
            // Create formula object from input text.
            TexFormula formula = null;
            try
            {
                formula = this.formulaParser.Parse(this.inputTextBox.Text);
            }
#if !DEBUG
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while parsing the given input:" + Environment.NewLine +
                    Environment.NewLine + ex.Message, "WPF-Math Example", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
#endif
            finally
            {
            }
            
            // Render formula to visual.
            var visual = new DrawingVisual();
            var renderer = formula.GetRenderer(TexStyle.Display, 20d);
            var formulaSize = renderer.RenderSize;

            using (var drawingContext = visual.RenderOpen())
            {
                renderer.Render(drawingContext, 0, 1);
            }

            this.formulaContainerElement.Visual = visual;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TexFormulaParser.Initialize();
            this.formulaParser = new TexFormulaParser();

            var testFormula1 = "\\int_0^{\\infty}{x^{2n} e^{-a x^2} dx} = \\frac{2n-1}{2a} \\int_0^{\\infty}{x^{2(n-1)} e^{-a x^2} dx} = \\frac{(2n-1)!!}{2^{n+1}} \\sqrt{\\frac{\\pi}{a^{2n+1}}}";
            var testFormula2 = "\\int_a^b{f(x) dx} = (b - a) \\sum_{n = 1}^{\\infty}  {\\sum_{m = 1}^{2^n  - 1} { ( { - 1} )^{m + 1} } } 2^{ - n} f(a + m ( {b - a}  )2^{-n} )";
            var testFormula3 = "L = \\int_a^b \\sqrt[4]{ |\\sum_{i,j=1}^ng_{ij}(\\gamma(t)) (\\frac{d}{dt}x^i\\circ\\gamma(t) ) (\\frac{d}{dt}x^j\\circ\\gamma(t) ) |}dt";
            this.inputTextBox.Text = testFormula3;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //
        }
    }
}
