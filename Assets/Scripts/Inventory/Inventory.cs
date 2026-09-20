using UnityEngine;

/// <summary>
/// Oyundaki tüm envanter yapılarının (PlayerInventory, WorkerInventory) ortak temel sınıfıdır.
/// Farklı aktör envanterlerinin IInteractable ve IItemSource sistemleri tarafından polimorfik olarak taşınabilmesini sağlar.
/// </summary>
public abstract class Inventory : MonoBehaviour
{
}

// daha genis kapsamli biur hale getirilmeli downcasting preoblemleri cozulmeli.