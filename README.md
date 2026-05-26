# PLC Copilot AI

AI-powered PLC automation assistant for generating SCL code, IO lists, safety analysis, and PDF reports.

> ⚠️ This project is currently the first public version (v1).  
> The developer plans to continue improving the system with advanced AI integrations, PLC simulation systems, ladder diagram generation, database support, and industrial automation features.

---

# Features

- PLC scenario analysis
- Automatic IO list generation
- SCL / Structured Text code generation
- Safety analysis system
- Timer and counter logic
- Alarm and emergency stop support
- HMI suggestions
- PDF export system
- Modern responsive UI
- Copy output system
- Dynamic automation logic generation

---

# Technologies Used

- ASP.NET MVC
- C#
- Razor Pages
- HTML
- CSS
- JavaScript
- QuestPDF

---

# Example Scenario

```text
4 motorlu konveyör sistemi istiyorum.
Sensör ürün görünce sistem çalışsın.
1. motor hemen çalışsın.
2. motor 5 saniye sonra çalışsın.
100 ürün sayılınca sistem dursun.
Alarm aktif olsun.
Acil stop olsun.
```

---

# Installation

## 1. Clone the repository

```bash
git clone https://github.com/emiri0/PlcCopilotAI.git
```

---

## 2. Open the project

Open the project using:

- Visual Studio 2022

---

## 3. Configure API Key

Open:

```text
appsettings.json
```

Replace:

```json
"ApiKey": "YOUR_API_KEY"
```

with your own OpenAI API key.

---

## 4. Install Required Packages

Install:

```text
QuestPDF
```

using NuGet Package Manager.

---

## 5. Run the project

Press:

```text
CTRL + F5
```

or run the project using Visual Studio.

---

# Roadmap

Planned future updates:

- OpenAI API integration
- Ladder diagram generation
- PLC simulation system
- Database integration
- User authentication system
- Project save/load system
- PLC brand selection
- Industrial automation assistant
- Real-time AI chatbot
- Advanced PDF/Excel reporting

---

# Screenshots

Coming soon...

---

# Author

Emir Gazi Yıldırım

GitHub:
https://github.com/emiir0
