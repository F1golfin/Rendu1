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
    private Graphe graphe;
    private Dictionary<int, SKPoint> positionsNoeuds;
    private Random random = new Random();

    /// <summary>
    /// Constructeur de la fenêtre principale
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        ChargerGrapheDepuisFichier("../../../../Files/soc-karate.txt");
        GenererPositionsNoeuds();
        string cheminFichier = "../../../../Files/soc-karate.txt";

        if (!File.Exists(cheminFichier))
        {
            Console.WriteLine($"Fichier '{cheminFichier}' introuvable");
            return;
        }

        string[] lignes = File.ReadAllLines(cheminFichier);
        int maxSommet = 0;
        List<(int, int)> liens = new List<(int, int)>();

        /// Lire les liens entre sommets et trouver le sommet maximum
        foreach (string ligne in lignes)
        {
            string ligneNettoyee = ligne.Trim(); /// Nettoyer les espaces inutiles

            if (string.IsNullOrWhiteSpace(ligneNettoyee))
                continue;


            Match match = Regex.Match(ligneNettoyee, @"\((\d+),\s*(\d+)\)");

            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int sommet1) &&
                    int.TryParse(match.Groups[2].Value, out int sommet2))
                {
                    liens.Add((sommet1, sommet2));
                    maxSommet = Math.Max(maxSommet, Math.Max(sommet1, sommet2)); /// Trouver le plus grand sommet
                }
                else
                {
                    Console.WriteLine($"Erreur de conversion sur la ligne : {ligne}");
                }
            }
            else
            {
                Console.WriteLine($"Ligne ignorée : '{ligne}' (format invalide)");
            }
        }

        /// Nombre total de sommets = sommet max + 1 (car on commence à 0)
        int nombreSommets = maxSommet + 1;
        Console.WriteLine($"Nombre de sommets : {nombreSommets}");

        Graphe graphe = new Graphe(nombreSommets);

        /// Ajouter les liens au graphe
        foreach (var (sommet1, sommet2) in liens)
        {
            graphe.AjouterLien(sommet1, sommet2);
        }

        /// Afficher les résultats
        Console.WriteLine("\n----------------------------LISTE D'ADJACENCE----------------------------\n");
        graphe.AfficherListeAdjacence();
        Console.WriteLine("\n----------------------------MATRICE D'ADJACENCE----------------------------\n");
        graphe.AfficherMatriceAdjacence();
        Console.WriteLine("\n----------------------------PARCOURS DE GRAPHE----------------------------\n");
        int sommet = -1;
        do
        {
            Console.WriteLine("Entrer un sommet de départ du parcours (compris entre 0 et "+graphe.NbSommets+") : ");
            sommet = int.Parse(Console.ReadLine());
        }while( sommet < 0 );   
        graphe.ParcoursEnProfondeur(sommet);
        graphe.ParcoursEnLargeur(sommet);
    }
    
    /// <summary>
    /// Génère les positions des nœuds sur le canvas en évitant les chevauchements
    /// </summary>
    private void GenererPositionsNoeuds()
    {
        positionsNoeuds = new Dictionary<int, SKPoint>();
        int largeurCanvas = 2000;
        int hauteurCanvas = 800;
        int marge = 50;
        int distanceMin = 40; /// Distance minimale entre deux sommets

        for (int i = 0; i < graphe.NbSommets; i++)
        {
            SKPoint newPosition;
            bool positionValide;
            int essais = 0;
            do
            {
                positionValide = true;
                newPosition = new SKPoint(
                    random.Next(marge, largeurCanvas - marge),
                    random.Next(marge, hauteurCanvas - marge)
                );

                /// Vérifie la distance avec les autres sommets
                foreach (var existingPosition in positionsNoeuds.Values)
                {
                    float dx = newPosition.X - existingPosition.X;
                    float dy = newPosition.Y - existingPosition.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (distance < distanceMin)
                    {
                        positionValide = false;
                        break;
                    }
                }
                essais++;
            } while (!positionValide && essais < 100); /// Limiter les tentatives pour éviter les boucles infinies

            positionsNoeuds[i] = newPosition;
        }
    }

    /// <summary>
    /// Gère l'affichage du graphe sur le canvas
    /// </summary>
    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        using (var paintLien = new SKPaint { Color = SKColors.Black, StrokeWidth = 2 })
        using (var paintNoeud = new SKPaint { Color = SKColors.Blue, IsAntialias = true })
        using (var paintTexte = new SKPaint { Color = SKColors.White, TextSize = 20, TextAlign = SKTextAlign.Center })
        {
            /// Dessiner les liens
            foreach (var (noeud, voisins) in graphe.ListeAdjacence)
            {
                foreach (var voisin in voisins)
                {
                    if (noeud < voisin) /// éviter les doublons
                    {
                        canvas.DrawLine(positionsNoeuds[noeud], positionsNoeuds[voisin], paintLien);
                    }
                }
            }

            /// Dessiner les noeuds
            foreach (var (id, position) in positionsNoeuds)
            {
                canvas.DrawCircle(position, 20, paintNoeud);
                canvas.DrawText(id.ToString(), position.X, position.Y + 7, paintTexte);
            }
        }
    }
    
    /// <summary>
    /// Charge un graphe depuis un fichier et le stocke dans la structure de données
    /// </summary>
    private void ChargerGrapheDepuisFichier(string cheminFichier)
    {
        if (!File.Exists(cheminFichier))
        {
            MessageBox.Show($"Fichier '{cheminFichier}' introuvable", "Erreur", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        string[] lignes = File.ReadAllLines(cheminFichier);
        int maxSommet = 0;
        List<(int, int)> liens = new List<(int, int)>();

        foreach (string ligne in lignes)
        {
            string ligneNettoyee = ligne.Trim();
            if (string.IsNullOrWhiteSpace(ligneNettoyee))
                continue;

            Match match = Regex.Match(ligneNettoyee, @"\((\d+),\s*(\d+)\)");
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int sommet1) &&
                    int.TryParse(match.Groups[2].Value, out int sommet2))
                {
                    liens.Add((sommet1, sommet2));
                    maxSommet = Math.Max(maxSommet, Math.Max(sommet1, sommet2));
                }
            }
        }

        graphe = new Graphe(maxSommet + 1);

        foreach (var (sommet1, sommet2) in liens)
        {
            graphe.AjouterLien(sommet1, sommet2);
        }
    }
}
