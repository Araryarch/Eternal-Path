```
EternalPath/
│
├── Game/ <-- Inti game: aturan, entitas, dan logika
│ ├── Entities/ <-- Pemain, musuh, peluru, dll.
│ ├── Interfaces/ <-- IRenderable, IMovable, ICollidable, dll.
│ └── Systems/ <-- Logika game: pergerakan, tabrakan, skor
│
├── Engine/ <-- Hal teknikal: rendering, asset loader
│ ├── Graphics/ <-- GDI+ renderer, sprite drawer
│ ├── Assets/ <-- Asset loader (gambar/suara)
│ └── Input/ <-- Penanganan input keyboard/mouse
│
├── UI/ <-- Windows Forms, tampilan utama
│ └── Eternal.cs <-- Form utama (game window)
│
├── Assets/ <-- Gambar, suara, JSON
│ ├── Sprites/
│ └── Sounds/
│
├── Program.cs <-- Entry point
└── GameLoop.cs <-- (Opsional) Loop utama untuk update/render
```
