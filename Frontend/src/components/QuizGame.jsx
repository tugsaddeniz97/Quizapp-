import React, { useEffect, useState } from "react";

function decodeHtmlEntities(value) {
  if (typeof value !== "string" || value.length === 0) {
    return "";
  }

  if (typeof window === "undefined") {
    return value;
  }

  const textarea = document.createElement("textarea");
  textarea.innerHTML = value;
  return textarea.value;
}

function normalizeText(value) {
  const decoded = decodeHtmlEntities(value);

  return decoded.replaceAll("^2", "²").replaceAll("^3", "³");
}

function getQuestionText(question) {
  if (!question || typeof question !== "object") {
    return "";
  }

  const rawText =
    question.question ||
    question.text ||
    question.title ||
    question.prompt ||
    "";

  return normalizeText(rawText);
}

function getOptions(question) {
  if (!question || typeof question !== "object") {
    return [];
  }

  const rawOptions =
    question.options || question.answers || question.choices || [];

  if (!Array.isArray(rawOptions)) {
    return [];
  }

  return rawOptions.map((option) => {
    if (typeof option === "string") {
      return normalizeText(option);
    }

    if (option && typeof option === "object") {
      const rawText = option.text || option.answerText || option.label || "";
      return normalizeText(rawText);
    }

    return "";
  });
}

export default function QuizGame({ questions = [], onRestart }) {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [selectedIndex, setSelectedIndex] = useState(null);
  const [answerResult, setAnswerResult] = useState(null);
  const [timeLeft, setTimeLeft] = useState(60);
  const minutes = Math.floor(timeLeft / 60);
  const seconds = timeLeft % 60;
  const totalQuestions = Math.min(questions.length, 10);
  const isComplete = totalQuestions > 0 && currentIndex >= totalQuestions;
  const currentQuestion = questions[currentIndex];
  const questionText = getQuestionText(currentQuestion);
  const options = getOptions(currentQuestion);
  const optionLabels = ["A", "B", "C", "D"];
  const category = normalizeText(
    currentQuestion?.category ||
      currentQuestion?.topic ||
      currentQuestion?.tag ||
      currentQuestion?.categoryName ||
      "",
  );

  useEffect(() => {
    if (totalQuestions === 0 || isComplete) {
      return;
    }

    setSelectedIndex(null);
    setAnswerResult(null);
  }, [currentIndex, totalQuestions, isComplete]);

  useEffect(() => {
    if (totalQuestions === 0 || isComplete || timeLeft <= 0) {
      return;
    }

    const intervalId = setInterval(() => {
      setTimeLeft((prev) => Math.max(prev - 1, 0));
    }, 1000);

    return () => clearInterval(intervalId);
  }, [timeLeft, totalQuestions, isComplete]);

  useEffect(() => {
    if (totalQuestions === 0 || isComplete || timeLeft !== 0) {
      return;
    }

    setCurrentIndex(totalQuestions);
  }, [timeLeft, totalQuestions, isComplete]);

  const handleAnswer = (index) => {
    setSelectedIndex(index);
    setAnswerResult(null);
  };

  const handleEndGame = () => {
    setCurrentIndex(totalQuestions);
  };

  const handleNext = () => {
    setCurrentIndex((prev) => Math.min(prev + 1, totalQuestions));
  };

  const handleRestart = () => {
    setCurrentIndex(0);
    setSelectedIndex(null);
    setAnswerResult(null);
    setTimeLeft(60);
    if (typeof onRestart === "function") {
      onRestart();
    }
  };

  const getOptionClasses = (index) => {
    const baseClasses =
      "text-white font-semibold px-6 py-3 rounded-lg transition";

    if (selectedIndex === index && answerResult === "correct") {
      return `${baseClasses} bg-green-500 hover:bg-green-600`;
    }

    if (selectedIndex === index && answerResult === "incorrect") {
      return `${baseClasses} bg-red-500 hover:bg-red-600`;
    }

    if (selectedIndex === index) {
      return `${baseClasses} bg-amber-400 hover:bg-amber-500 text-slate-900`;
    }

    return `${baseClasses} bg-cyan-500 hover:bg-cyan-600`;
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-red-100">
      <div className="relative h-[70vh] w-[50vw] bg-white p-10 rounded-xl text-center border-2 border-cyan-400 shadow-[0_0_20px_rgba(34,211,238,0.8)]">
        {totalQuestions === 0 ? (
          <div className="flex h-full items-center justify-center text-gray-600">
            Loading questions...
          </div>
        ) : isComplete ? (
          <div className="flex h-full items-center justify-center">
            <div className="space-y-4">
              <div className="text-2xl font-semibold text-cyan-600">
                Quiz complete!
              </div>
              <div className="text-gray-600">You finished all questions.</div>
              <button
                className="rounded-lg bg-cyan-500 px-6 py-2 text-sm font-semibold text-white transition hover:bg-cyan-600"
                onClick={handleRestart}
              >
                Start new quiz
              </button>
            </div>
          </div>
        ) : (
          <div className="flex h-full flex-col">
            <button
              className="absolute left-6 top-6 rounded-lg border border-cyan-200 px-4 py-2 text-sm font-semibold text-cyan-700 transition hover:border-cyan-300 hover:bg-cyan-50"
              onClick={handleEndGame}
            >
              End game
            </button>
            <div
              className={`absolute right-6 top-6 text-lg font-semibold ${
                timeLeft === 0 ? "text-red-500" : "text-cyan-700"
              }`}
            >
              {String(minutes).padStart(2, "0")}:
              {String(seconds).padStart(2, "0")}
            </div>
            <div className="text-sm text-gray-500 mb-2">
              Question {currentIndex + 1} of {totalQuestions}
            </div>
            <div className="flex flex-1 flex-col items-center justify-center">
              <div className="text-lg font-semibold uppercase tracking-wide text-teal-500">
                {category || "General"}
              </div>
              <div className="mt-4 text-4xl font-semibold text-gray-800">
                {questionText || "Question text is missing."}
              </div>
            </div>
            <div className="mt-auto pt-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                {optionLabels.map((label, index) => (
                  <button
                    key={label}
                    className={getOptionClasses(index)}
                    onClick={() => handleAnswer(index)}
                  >
                    {label}
                    {options[index] ? `: ${options[index]}` : ""}
                  </button>
                ))}
              </div>
              <div className="flex justify-end">
                <button
                  className="rounded-lg border border-cyan-200 px-6 py-2 text-sm font-semibold text-cyan-700 transition hover:border-cyan-300 hover:bg-cyan-50"
                  onClick={handleNext}
                >
                  Next
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
