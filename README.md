# 🐝 The Bee
> **A 4th-Year University Game Development Assignment**

**The Bee** is a 3D rogue-lite flight simulator that explores the life of a honey bee through physics-based gameplay. Players must balance risk vs. reward by managing flight stamina (nectar) and inventory weight (pollen) while navigating a semi-procedural meadow environment.

---

## 🎮 Gameplay Overview
**Survive the Meadow. Fill the Hive.**

Take flight as a tiny bee in a massive world. The Bee is a relaxing yet strategic flying game where you explore a lush, dangerous meadow.

*   **Fly Fast, Carry Heavy:** The more pollen you collect, the slower you fly. Do you push your luck for a high score, or return home safe?
*   **Manage Your Fuel:** Nectar is your stamina. Run out, and you're grounded.
*   **Macro World:** Experience the world from a bug's eye view with immersive depth-of-field visuals and spatial sound.

---

## 🔑 Key Features

*   **Physics-Based Flight Controller**  
    Custom Rigidbody controller with drag, angular damping, and hover mechanics.

*   **Dynamic Weight System**  
    Collecting pollen increases mass, dynamically altering flight handling and speed (simulating real bee physics).

*   **Resource Management Loop**  
    A "Rogue-lite" core loop where players must balance gathering resources against a draining stamina bar before returning to the hive.

*   **Immersive Audio**  
    Features procedural engine sounds (pitch-shifting based on velocity) and 3D spatial audio for landmarks.

*   **Universal Render Pipeline (URP)**  
    Utilizes custom shaders for wind-affected vegetation and macro-style depth of field.

---

## 🛠️ Tech Stack

| Category | Technology |
| :--- | :--- |
| **Engine** | Unity 2022.3 (URP) |
| **Language** | C# |
| **Tools** | VS Code |

---

### 📊 Live Feedback Stats
![Feedback Count](https://img.shields.io/github/issues-search/MagixIsAvailable/Bee-Test?query=label%3Aplaytest-feedback&label=Total%20Playtests&color=orange)
![Bugs Reported](https://img.shields.io/github/issues-search/MagixIsAvailable/Bee-Test?query=label%3Abug&label=Bugs%20Found&color=red)