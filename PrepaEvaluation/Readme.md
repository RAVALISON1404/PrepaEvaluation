# Préparation évaluation 2024

Ces instructions garantissent une manipulation sûre et précise des données dans l'application.

## I. Modification

### 1. Champ ID Obligatoire:

- Lors de la modification d'un élément, assurez-vous de remplir le champ ID, qui est désormais obligatoire. Cela garantit la manipulation précise des données.

### 2. Concordance des Noms de Champs dans le Modal

- Pour une modification correcte des données, veillez à ce que les noms des champs dans les formulaires correspondent exactement aux attributs de la classe associée.

### 3. Bouton de Modification

- Vérifiez que le bouton de modification est correctement associé à l'élément à modifier en consultant les attributs `data-url` et `data-id` :
    - `data-url` doit contenir l'URL pour récupérer les données, par exemple `/Home/GetById`.
    - `data-id` doit contenir l'identifiant de l'élément à modifier.
- L'utilisation de la fonction `updateItem(this)` est obligatoire.

## II. Suppression

- Assurez-vous que le bouton de suppression est correctement associé à l'élément à supprimer en consultant les attributs `data-url` et `data-id` :
    - `data-url` doit contenir l'URL pour la suppression, par exemple `/Home/Delete`.
    - `data-id` doit contenir l'identifiant de l'élément à supprimer.
- L'utilisation de la fonction `deleteItem(this)` est obligatoire.
- Utilisation de la méthode HTTP `DELETE` pour supprimer un élément.

## III. Utilisation des Séparateurs de Milliers en .NET

Pour formater un nombre avec des séparateurs de milliers, utilisez la méthode `ToString()` avec le format `"N"`.

### Exemple

```csharp
int number = 1000000;
string formattedNumber = number.ToString("N0"); // "1,000,000"
```
Le format `N0` affiche un nombre sans décimales.
### Exemple
```csharp
double number = 1234567.89;
string formattedNumber = number.ToString("N2"); // "1,234,567.89"
```
Le format `N2` affiche un nombre avec deux décimales.

## IV. Import

### 1. CSV :

- Les colonnes doivent être obligatoires, mais elles ne sont pas obligatoirement complètes.

## V. Export
### 1. CSV :
### 2. PDF :

- il faut mettre le dossier `Rotativa` dans wwwroot
- configurer rotativa dans Program.cs