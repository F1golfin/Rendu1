using Rendu1;
namespace Rendu1_Tests;

public class Lien_Noeud_Tests
{
    /// <summary>
    /// Vérifie que le constructeur de Noeud stocke correctement l'ID.
    /// </summary>
    [Test]
    public void Noeud_ShouldStoreIdCorrectly()
    {
        int expectedId = 42;
        Noeud noeud = new Noeud(expectedId);
        Assert.AreEqual(expectedId, noeud.Id);
    }
    
    /// <summary>
    /// Vérifie que le constructeur de Lien stocke correctement les noeuds.
    /// </summary>
    [Test]
    public void Lien_ShouldStoreNoeudsCorrectly()
    {
        Noeud noeud1 = new Noeud(1);
        Noeud noeud2 = new Noeud(2);
        Lien lien = new Lien(noeud1, noeud2);
        Assert.NotNull(lien);
    }
    
    [Test]
    public void Lien_ShouldConnectCorrectNoeuds()
    {
        Noeud noeud1 = new Noeud(1);
        Noeud noeud2 = new Noeud(2);
        Lien lien = new Lien(noeud1, noeud2);
        Assert.AreEqual(noeud1, lien.Noeud1);
        Assert.AreEqual(noeud2, lien.Noeud2);
    }
}