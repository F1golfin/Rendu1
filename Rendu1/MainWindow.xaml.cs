using System;
using System.Windows;
using System.IO;
using System.Text.RegularExpressions;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Rendu1;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        string cheminFichier = "soc-karate.txt";
        
        if (!File.Exists(cheminFichier))
        {
            Console.WriteLine($"⚠ Erreur : Le fichier '{cheminFichier}' n'existe pas !");
            return;
        }

        string[] lignes = File.ReadAllLines(cheminFichier);
        int maxSommet = 0;
        List<(int, int)> liens = new List<(int, int)>();

        // Lire les liens entre sommets et trouver le sommet maximum
        foreach (string ligne in lignes)
        {
            string ligneNettoyee = ligne.Trim(); // Nettoyer les espaces inutiles
            
            if (string.IsNullOrWhiteSpace(ligneNettoyee))
                continue;


            Match match = Regex.Match(ligneNettoyee, @"\((\d+),\s*(\d+)\)");

            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int sommet1) &&
                    int.TryParse(match.Groups[2].Value, out int sommet2))
                {
                    liens.Add((sommet1, sommet2));

                    // Trouver le plus grand sommet
                    maxSommet = Math.Max(maxSommet, Math.Max(sommet1, sommet2));
                }
                else
                {
                    Console.WriteLine($"⚠ Erreur de conversion sur la ligne : {ligne}");
                }
            }
            else
            {
                Console.WriteLine($"⚠ Ligne ignorée : '{ligne}' (format invalide)");
            }
        }

        // Nombre total de sommets = sommet max + 1 (car on commence à 0)
        int nombreSommets = maxSommet + 1;
        Console.WriteLine($"✅ Nombre de sommets détecté : {nombreSommets}");

        Graphe graphe = new Graphe(nombreSommets);

        // Ajouter les liens au graphe
        foreach (var (sommet1, sommet2) in liens)
        {
            graphe.AjouterLien(sommet1, sommet2);
        }

        // Afficher les résultats
        graphe.AfficherListeAdjacence();
        graphe.AfficherMatriceAdjacence();
        
        
        InitializeComponent();
        
    }
    
    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        using (var paint = new SKPaint())
        {
            paint.Color = SKColors.Blue;
            paint.IsAntialias = true;
            paint.Style = SKPaintStyle.Fill;
            paint.StrokeWidth = 5;

            canvas.DrawCircle(100, 100, 50, paint);
        }
    }
}