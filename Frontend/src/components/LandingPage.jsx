import React from "react";

export default function LandingPage() {
  const handlePlayClick = () => {
    console.log("Play button clicked");
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-red-100">
      <div className="bg-white p-10 rounded-xl shadow-lg text-center">
        <h1 className="text-4xl font-bold text-red-600 mb-4">Quiz App</h1>
        <p className="text-gray-700 mb-6">
          Test your knowledge with our interactive quizzes
        </p>
        <button
          className="bg-red-500 hover:bg-red-600 text-white font-semibold px-6 py-3 rounded-lg transition"
          onClick={handlePlayClick}
        >
          Play
        </button>
      </div>
    </div>
  );
}
