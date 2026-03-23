# Concurrent Programming - Billiard Table

Program przedstawia kule poruszajace sie po prostokatnym stole bilardowym. Uzytkownik podaje liczbe kul, a aplikacja uruchamia symulacje z odbiciami od scian oraz oddzialywaniami miedzy kulami.

## Repository

Repository URL: https://github.com/wspolbiegi-DW-KS/Wspolbiegi

## Working Group

| Name Surname | GUID |
| --- | --- |
| Dawid Wachecki | {d19a1746-c774-4256-8216-be3cdd9c0d1a} |
| Kacper Skoczylas | {3c86ebe5-6500-4ca7-90b8-4e3a05f349ad} |

## Layered Architecture

Program jest podzielony na warstwy zapisane terminami jezyka programowania:

1. Data layer (`Data`)
2. Logic layer (`BusinessLogic`)
3. Presentation layer (`PresentationView`, `PresentationViewModel`, `PresentationModel`)

### Data Layer Responsibility

Warstwa `Data` reprezentuje kule (`BallEntity`) i przechowuje ich stan w repozytorium (`IBallRepository`, `InMemoryBallRepository`).

### Presentation Layer Responsibility

Warstwa `Presentation` udostepnia GUI WPF i pozwala podac liczbe kul, pokazuje ich pozycje poczatkowe na stole oraz wizualizuje ruch w czasie.

## Build And Tests

Uruchomienie kompilacji:

```powershell
dotnet build .\Wspolbiegi.sln -c Debug
```

Uruchomienie testow jednostkowych:

```powershell
dotnet test .\Wspolbiegi.sln -c Debug
```

Uruchomienie aplikacji:

```powershell
dotnet run --project .\PresentationView\PresentationView.csproj -c Debug
```