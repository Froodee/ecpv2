# Module Promo/Réduction - Architecture MVVM

## Vue d'ensemble
Module complet de gestion des promos/réductions pour l'application ECPV2 (WinUI).
Implémente le pattern MVVM avec RelayCommand et databinding bidirectionnel.

## Structure des fichiers

```
Features/promo/
├── ViewModel/
│   ├── IPromoViewModel.cs      (Interface du ViewModel)
│   └── PromoViewModel.cs       (Implémentation MVVM)
└── View/
    ├── PromoPage.xaml          (Page principale)
    ├── PromoPage.xaml.cs
    ├── PromoSearchControl.xaml (Barre de recherche)
    ├── PromoSearchControl.xaml.cs
    ├── PromoListControl.xaml   (Liste des promos)
    ├── PromoListControl.xaml.cs
    ├── PromoEditControl.xaml   (Formulaire d'édition)
    ├── PromoEditControl.xaml.cs
    ├── PromoButtonControl.xaml (Boutons CRUD)
    └── PromoButtonControl.xaml.cs
```

## ViewModel

### IPromoViewModel.cs
Interface publique exposant:
- **Propriétés:**
  - `PromoSearch: string` - Critère de recherche
  - `Promos: ObservableCollection<Promo>` - Liste des promos
  - `PromoSelected: Promo?` - Promo sélectionnée
  - `IsEditable: bool` - État d'édition

- **Commandes (ICommand):**
  - `CommandPromoNew` - Créer une nouvelle promo
  - `CommandPromoEdit` - Passer en mode édition
  - `CommandPromoSave` - Enregistrer les modifications
  - `CommandPromoDelete` - Supprimer la promo sélectionnée
  - `CommandPromoSearch` - Rechercher des promos
  - `CommandPromoCancel` - Annuler l'édition

### PromoViewModel.cs
Implémentation du ViewModel:
- Hérite de `BaseViewModel` (gère les events INotifyPropertyChanged)
- Utilise `EcpContext` pour l'accès à la base de données
- Chaque commande a une méthode `Action*` (exécution) et `Can*` (validation)

**Méthodes principales:**
- `ActionPromoNew()` - Crée une nouvelle instance Promo
- `ActionPromoEdit()` - Active le mode édition (IsEditable = true)
- `ActionPromoSave()` - Ajoute ou met à jour une promo en DB
- `ActionPromoDelete()` - Supprime une promo de la DB
- `ActionPromoSearch()` - Filtre les promos par réduction
- `ActionPromoCancel()` - Annule les modifications et rechargement

**Gestion des erreurs:**
- Try-catch sur toutes les opérations
- `LogException()` pour tracer les erreurs
- Initialisation gracieuse en cas d'erreur

## Views

### PromoPage.xaml
Page principale composée d'une grille 3x2:
```
[      Barre de Recherche (3 colonnes)     ]
[Liste] [      Formulaire d'Édition      ] [Boutons]
```

### PromoSearchControl
Contrôle de recherche:
- TextBox lié à `PromoSearch`
- Bouton "Rechercher" exécutant `CommandPromoSearch`
- Recherche par valeur de réduction

### PromoListControl
ListView affichant les promos:
- Binding: `ItemsSource="{Binding Promos, Mode=TwoWay}"`
- Template affichant: ID et Réduction (%)
- Selection liée à `PromoSelected`

### PromoEditControl
Formulaire avec 4 champs:
1. **Idpromo** (TextBox désactivé) - Clé primaire
2. **Datedeb** (TextBox) - Date de début (format YYYY-MM-DD)
3. **Datefin** (TextBox) - Date de fin (format YYYY-MM-DD)
4. **Reduc** (TextBox) - Pourcentage de réduction

Tous les champs sont liés à `PromoSelected` avec `IsEnabled="{Binding IsEditable}"`

### PromoButtonControl
5 boutons CRUD en StackPanel vertical:
- **Nouveau** → `CommandPromoNew`
- **Éditer** → `CommandPromoEdit`
- **Enregistrer** → `CommandPromoSave`
- **Supprimer** → `CommandPromoDelete`
- **Annuler** → `CommandPromoCancel`

## Entité Promo

```csharp
public class Promo
{
    public short Idpromo { get; set; }           // Clé primaire
    public DateOnly Datedeb { get; set; }        // Date début
    public DateOnly Datefin { get; set; }        // Date fin
    public decimal Reduc { get; set; }           // Réduction (%)
    public ICollection<Produit> Produits { get; set; } // Produits concernés
}
```

## Flux d'utilisation

1. **Chargement initial:**
   - PromoViewModel se crée et charge toutes les promos via EcpContext
   - Les promos sont affichées dans la liste

2. **Créer une promo:**
   - Clic sur "Nouveau" → ActionPromoNew() 
   - Nouvelle instance vierge créée
   - IsEditable = true → Formulaire actif
   - Remplir les champs → Clic "Enregistrer" → ActionPromoSave()
   - Nouvelle promo ajoutée en DB

3. **Éditer une promo:**
   - Sélectionner une promo dans la liste
   - Clic "Éditer" → IsEditable = true
   - Modifier les champs
   - Clic "Enregistrer" → ActionPromoSave() (Update DB)
   - ou "Annuler" → Reload depuis DB

4. **Supprimer une promo:**
   - Sélectionner une promo
   - Clic "Supprimer" → ActionPromoDelete()
   - Promo supprimée de la DB
   - Liste rechargée

5. **Rechercher:**
   - Entrer valeur de réduction
   - Clic "Rechercher" → ActionPromoSearch()
   - Liste filtrée affichée

## Points techniques importants

- **SetProperty()** - Utilisé pour les propriétés avec notification de changement
- **ObservableCollection** - Pour la liaison dynamique des listes
- **TwoWay Binding** - Synchronisation bidirectionnelle UI ↔ ViewModel
- **RelayCommand** - Implémentation ICommand avec actions et validations
- **EcpContext** - DbContext EF Core pour l'accès DB
- **Include()** - Chargement eager de la collection Produits

## Namespaces utilisés

- `ECPV2.Features.promo.View` - Contrôles XAML
- `ECPV2.Features.promo.ViewModel` - Logique métier
- `ECPV2.Domain.Models` - Entité Promo
- `ECPV2.Core.ViewModels` - BaseViewModel
- `ECPV2.Services.Command` - RelayCommand

## Pattern suivi

Architecture identique aux modules existants:
- ✓ Client
- ✓ Employe
- ✓ Admin

Assure une cohérence et maintenabilité du codebase.
