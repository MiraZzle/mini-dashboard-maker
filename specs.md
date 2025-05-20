# Mini Dashboard Maker

Jedná se o lokálně nasaditelnou Blazor aplikaci Mini Dashboard Maker. Aplikace umožní uživateli registrovat různé datové zdroje (např. REST API, URL ke stažení souborů nebo webové stránky pro crawling). Tyto zdroje bude možné uložit a následně z nich vytvářet vlastní dashboardy s jednoduchými vizualizacemi dat. Každý dashboard bude mít možnost nastavení automatické obnovy dat v definovaném intervalu a přizpůsobení rozložení. Veškerá konfigurace bude perzistována v lokální databázi nebo pomocí JSON souborů.

Použité technologie:

- delegates
- asynchronní komunikace (async/await)
- multithreading (periodické načítání dat)
- LINQ
- polymorfismus a interfaces
- regulární výrazy
- práce se soubory nebo SQLite pro lokální úložiště
