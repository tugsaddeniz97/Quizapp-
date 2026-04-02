# 🎯 QuizApp

A full-stack quiz application built with **ASP.NET Core 10** (backend) and **React** (frontend). Players can answer trivia questions, earn points based on speed and accuracy, and compete on leaderboards.

## 📋 Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Testing](#testing)
- [Project Structure](#project-structure)

## ✨ Features

- 🎮 **Interactive Quiz Gameplay** - Answer multiple-choice questions with time tracking
- 🏆 **Dynamic Scoring System** - Earn points based on correctness and speed (base 10 points + bonus for fast answers)
- 👤 **Player Management** - Create and manage player profiles
- 📚 **Question Bank** - Import and manage trivia questions from JSON files
- 🎲 **Randomized Questions** - Get random sets of questions for each game session
- 📊 **Score Tracking** - Complete history of all player attempts and scores
- 🔄 **RESTful API** - Clean API architecture for easy frontend integration
- ✅ **Comprehensive Testing** - Unit tests for all controllers using xUnit

## 🛠️ Tech Stack

**Backend:**
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQLite
- xUnit & Moq (Testing)

**Frontend:**
- React
- Vite (Development server on port 5173)

**Tools:**
- CORS configured for React frontend
- In-Memory Database for testing

## 🏗️ Architecture

The application follows a 3-tier architecture with Controllers (API), Business Logic (Mappers/DTOs), and Data Access (EF Core + SQLite).

**Key Components:**
- **Models**: Player, Question, Score (database entities)
- **DTOs**: QuestionDTO, PlayQuestionDTO, PlayResponse (API contracts)
- **Mappers**: QuestionMapper (entity-DTO conversion)
- **Controllers**: GameController, PlayerController, QuestionsController

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js (v18+)
- Git

### Backend Setup

1. Clone the repository
2. Navigate to the QuizApp folder
3. Run `dotnet restore`
4. Run `dotnet ef database update` to create quiz.db
5. Run `dotnet run` (API available at http://localhost:5241)

### Frontend Setup

1. Navigate to your frontend folder
2. Run `npm install`
3. Run `npm run dev` (React app on http://localhost:7239)

Note: CORS is configured for localhost:5173. Update Program.cs if using a different port.

## 📡 API Endpoints

### Players
- `POST /Player` - Create player (body: `{"userName": "string"}`)
- `GET /Player` - Get all players

### Questions
- `GET /Questions` - Get all questions
- `POST /Questions` - Add question (body: QuestionDTO)
- `POST /Questions/import` - Import from questionsData.json

### Gameplay
- `GET /play/questions?count=10` - Get random questions
- `POST /play` - Submit answer (body: `{"playerId": 1, "questionId": 5, "answer": "Paris", "timeTakenSeconds": 8}`)

**Scoring:** Base 10 points + max(0, 5 - timeTakenSeconds) bonus

## 🗄️ Database Schema

**Players:** PlayerId, Username, CreatedAt, TotalScore

**Questions:** Id, Type, Difficulty, Category, Text, CorrectAnswer, IncorrectAnswersJson

**Scores:** Id, PlayerId (FK), QuestionId (FK), IsCorrect, Points, TimeTakenSeconds, CreatedAt

**Relationships:** Player 1:N Score, Question 1:N Score

## 🧪 Testing

Run tests with `dotnet test` from QuizApp.Tests folder.

Tests use in-memory database and follow AAA pattern (Arrange-Act-Assert).

## 📁 Project Structure
