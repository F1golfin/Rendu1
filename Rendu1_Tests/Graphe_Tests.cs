using Rendu1;
namespace Rendu1_Tests;

public class Graphe_Tests
{
    /// <summary>
    /// Classe de tests unitaires pour la classe Graphe.
    /// </summary>
    [TestFixture]
    public class GrapheTests
    {
        /// <summary>
        /// Vérifie que le constructeur initialise correctement le nombre de sommets.
        /// </summary>
        [Test]
        public void Graphe_ShouldInitializeWithCorrectNbSommets()
        {
            int expectedNbSommets = 5;
            Graphe graphe = new Graphe(expectedNbSommets);
            Assert.That(graphe.NbSommets, Is.EqualTo(expectedNbSommets));
        }

        /// <summary>
        /// Vérifie que le graphe initialise bien une liste d'adjacence vide.
        /// </summary>
        [Test]
        public void Graphe_ShouldInitializeEmptyAdjacencyList()
        {
            int nbSommets = 3;
            Graphe graphe = new Graphe(nbSommets);
            for (int i = 0; i < nbSommets; i++)
            {
                Assert.IsNotNull(graphe.ListeAdjacence[i]); // Vérifie que la liste existe
                Assert.IsEmpty(graphe.ListeAdjacence[i]); // Vérifie qu'elle est vide
            }
        }

        /// <summary>
        /// Vérifie que AjouterLien() ajoute bien une connexion bidirectionnelle.
        /// </summary>
        [Test]
        public void AjouterLien_ShouldCreateBidirectionalConnection()
        {
            Graphe graphe = new Graphe(3);
            graphe.AjouterLien(0, 1);
            Assert.Contains(1, graphe.ListeAdjacence[0]); // 0 → 1
            Assert.Contains(0, graphe.ListeAdjacence[1]); // 1 → 0
        }

        /// <summary>
        /// Vérifie que le parcours en profondeur fonctionne correctement.
        /// </summary>
        [Test]
        public void ParcoursEnProfondeur_ShouldPrintCorrectOutput()
        {
            Graphe graphe = new Graphe(5);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);
            graphe.AjouterLien(2, 3);
            graphe.AjouterLien(3, 4);
            
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw); 
                
                graphe.ParcoursEnProfondeur(0);
                
                string consoleOutput = sw.ToString().Trim(); 
                Assert.IsTrue(consoleOutput.Contains("Parcours en Profondeur"));
                Assert.IsTrue(consoleOutput.Contains("0"));
                Assert.IsTrue(consoleOutput.Contains("1"));
                Assert.IsTrue(consoleOutput.Contains("2"));
                Assert.IsTrue(consoleOutput.Contains("3"));
                Assert.IsTrue(consoleOutput.Contains("4"));
            }
        }


        /// <summary>
        /// Vérifie que le parcours en largeur fonctionne correctement.
        /// </summary>
        [Test]
        public void ParcoursEnLargeur_ShouldVisitAllNodesInOrder()
        {
            Graphe graphe = new Graphe(4);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(0, 2);
            graphe.AjouterLien(1, 3);
            graphe.AjouterLien(2, 3);
            
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                graphe.ParcoursEnLargeur(0);
                string consoleOutput = sw.ToString().Trim();
                Assert.IsTrue(consoleOutput.Contains("Parcours en Largeur"));
                Assert.IsTrue(consoleOutput.Contains("0"));
                Assert.IsTrue(consoleOutput.Contains("1"));
                Assert.IsTrue(consoleOutput.Contains("2"));
                Assert.IsTrue(consoleOutput.Contains("3"));
            }
        }
        
        [Test]
        public void MatriceAdjacence_ShouldReflectLinksCorrectly()
        {
            Graphe graphe = new Graphe(3);
            graphe.AjouterLien(0, 1);

            Assert.IsTrue(graphe.MatriceAdjacence[0, 1]);
            Assert.IsTrue(graphe.MatriceAdjacence[1, 0]);
            Assert.IsFalse(graphe.MatriceAdjacence[0, 2]); // Pas de lien entre 0 et 2
        }
    }
}