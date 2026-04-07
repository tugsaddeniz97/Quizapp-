import React, { useEffect, useMemo, useState } from "react";
import { submitAnswer } from "../services/api";

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

export default function QuizGame({
  questions = [],
  onRestart,
  playerId,
  players = [],
  onRefreshPlayers,
}) {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [selectedIndex, setSelectedIndex] = useState(null);
  const [answerResult, setAnswerResult] = useState(null);
  const [correctIndex, setCorrectIndex] = useState(null);
  const [correctAnswerDisplay, setCorrectAnswerDisplay] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [timeLeft, setTimeLeft] = useState(60);
  const [questionStartedAt, setQuestionStartedAt] = useState(Date.now());
  const [showHelp, setShowHelp] = useState(false);
  const [hasStarted, setHasStarted] = useState(false);
  const [quizPoints, setQuizPoints] = useState(0);
  const [latestTotalScore, setLatestTotalScore] = useState(null);
  const [didRefreshRank, setDidRefreshRank] = useState(false);
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
  const questionId =
    currentQuestion?.id ||
    currentQuestion?.questionId ||
    currentQuestion?.questionID ||
    null;
  const showCorrectAnswerText =
    answerResult === "incorrect" &&
    correctIndex === null &&
    Boolean(correctAnswerDisplay);

  const rankedPlayers = useMemo(() => {
    return [...players]
      .filter((player) => player)
      .sort((a, b) => (b.totalScore || 0) - (a.totalScore || 0));
  }, [players]);

  const playerRank = useMemo(() => {
    if (!playerId || rankedPlayers.length === 0) {
      return null;
    }

    const rankIndex = rankedPlayers.findIndex(
      (player) => player.playerId === playerId,
    );

    return rankIndex >= 0 ? rankIndex + 1 : null;
  }, [rankedPlayers, playerId]);

  useEffect(() => {
    if (totalQuestions === 0 || isComplete) {
      return;
    }

    setSelectedIndex(null);
    setAnswerResult(null);
    setCorrectIndex(null);
    setCorrectAnswerDisplay("");
    setIsSubmitting(false);
    if (hasStarted) {
      setQuestionStartedAt(Date.now());
    }
    setDidRefreshRank(false);
  }, [currentIndex, totalQuestions, isComplete, hasStarted]);

  useEffect(() => {
    if (!isComplete || didRefreshRank) {
      return;
    }

    if (typeof onRefreshPlayers === "function") {
      onRefreshPlayers();
    }
    setDidRefreshRank(true);
  }, [isComplete, didRefreshRank, onRefreshPlayers]);

  useEffect(() => {
    const handleKeyDown = (event) => {
      if (event.key.toLowerCase() === "h") {
        setShowHelp((prev) => !prev);
      }
    };

    window.addEventListener("keydown", handleKeyDown);
    return () => window.removeEventListener("keydown", handleKeyDown);
  }, []);

  useEffect(() => {
    if (totalQuestions === 0 || isComplete || timeLeft <= 0 || !hasStarted) {
      return;
    }

    const intervalId = setInterval(() => {
      setTimeLeft((prev) => Math.max(prev - 1, 0));
    }, 1000);

    return () => clearInterval(intervalId);
  }, [timeLeft, totalQuestions, isComplete, hasStarted]);

  useEffect(() => {
    if (totalQuestions === 0 || isComplete || timeLeft !== 0 || !hasStarted) {
      return;
    }

    setCurrentIndex(totalQuestions);
  }, [timeLeft, totalQuestions, isComplete, hasStarted]);

  const handleAnswer = async (index) => {
    if (
      isSubmitting ||
      answerResult !== null ||
      timeLeft === 0 ||
      !hasStarted ||
      !playerId ||
      !questionId
    ) {
      return;
    }

    setSelectedIndex(index);
    setIsSubmitting(true);

    const answerText = options[index] || "";
    const timeTakenSeconds = Math.max(
      0,
      Math.round((Date.now() - questionStartedAt) / 1000),
    );

    try {
      const response = await submitAnswer({
        playerId,
        questionId,
        answer: answerText,
        timeTakenSeconds,
      });

      const isCorrect = Boolean(response?.correct);
      setAnswerResult(isCorrect ? "correct" : "incorrect");
      setQuizPoints((prev) => prev + (response?.pointsEarned || 0));
      if (typeof response?.totalScore === "number") {
        setLatestTotalScore(response.totalScore);
      }

      const responseCorrectAnswer = normalizeText(
        response?.correctAnswer || "",
      );
      setCorrectAnswerDisplay(responseCorrectAnswer);

      if (!isCorrect && responseCorrectAnswer) {
        const normalizedCorrect = responseCorrectAnswer.trim().toLowerCase();
        const correctOptionIndex = options.findIndex(
          (option) => option.trim().toLowerCase() === normalizedCorrect,
        );
        setCorrectIndex(correctOptionIndex >= 0 ? correctOptionIndex : null);
      }
    } catch (error) {
      console.error("Failed to submit answer:", error);
      setAnswerResult("incorrect");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleEndGame = () => {
    setCurrentIndex(totalQuestions);
    setDidRefreshRank(true);
    if (typeof onRefreshPlayers === "function") {
      onRefreshPlayers();
    }
  };

  const handleNext = () => {
    setCurrentIndex((prev) => Math.min(prev + 1, totalQuestions));
  };

  const handleRestart = () => {
    setCurrentIndex(0);
    setSelectedIndex(null);
    setAnswerResult(null);
    setCorrectIndex(null);
    setCorrectAnswerDisplay("");
    setIsSubmitting(false);
    setTimeLeft(60);
    setQuestionStartedAt(Date.now());
    setQuizPoints(0);
    setLatestTotalScore(null);
    setDidRefreshRank(false);
    setHasStarted(false);
    setShowHelp(false);
    if (typeof onRestart === "function") {
      onRestart();
    }
  };

  const handleStartQuiz = () => {
    setHasStarted(true);
    setShowHelp(false);
    setTimeLeft(60);
    setQuestionStartedAt(Date.now());
  };

  const getOptionClasses = (index) => {
    const baseClasses =
      "text-white font-semibold px-6 py-3 rounded-lg transition";

    if (selectedIndex === index && answerResult === "correct") {
      return `${baseClasses} bg-green-500 hover:bg-green-600`;
    }

    if (answerResult === "incorrect") {
      if (index === correctIndex) {
        return `${baseClasses} bg-green-500 hover:bg-green-600`;
      }

      if (selectedIndex === index) {
        return `${baseClasses} bg-red-500 hover:bg-red-600`;
      }
    }

    if (selectedIndex === index) {
      return `${baseClasses} bg-amber-400 hover:bg-amber-500 text-slate-900`;
    }

    return `${baseClasses} bg-cyan-500 hover:bg-cyan-600`;
  };

  return (
    <div className="flex items-center justify-center">
      <div className="relative h-[70vh] w-full max-w-4xl bg-white p-10 rounded-xl text-center border-2 border-cyan-400 shadow-[0_0_20px_rgba(34,211,238,0.8)]">
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
              <div className="rounded-lg border border-emerald-100 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">
                Points this quiz:{" "}
                <span className="font-semibold">{quizPoints}</span>
              </div>
              <div className="text-sm text-gray-600">
                Overall rank: {playerRank ? `#${playerRank}` : "-"}
              </div>
              {typeof latestTotalScore === "number" ? (
                <div className="text-sm text-gray-600">
                  Total score: {latestTotalScore}
                </div>
              ) : null}
              <button
                className="rounded-lg bg-cyan-500 px-6 py-2 text-sm font-semibold text-white transition hover:bg-cyan-600"
                onClick={handleRestart}
              >
                Start new quiz
              </button>
            </div>
          </div>
        ) : !hasStarted ? (
          <div className="flex h-full flex-col items-center justify-center gap-6">
            <div className="text-sm uppercase tracking-[0.2em] text-cyan-500">
              Click play to begin
            </div>
            <button
              className="rounded-3xl bg-cyan-500 px-12 py-6 text-2xl font-semibold text-white shadow-[0_0_30px_rgba(34,211,238,0.6)] transition hover:bg-cyan-600"
              onClick={handleStartQuiz}
            >
              Play
            </button>
            <button
              className="rounded-lg border border-cyan-200 px-4 py-2 text-sm font-semibold text-cyan-700 transition hover:border-cyan-300 hover:bg-cyan-50"
              onClick={() => setShowHelp(true)}
            >
              Help
            </button>
          </div>
        ) : (
          <div className="flex h-full flex-col">
            <button
              className="absolute left-6 top-6 rounded-lg border border-cyan-200 px-4 py-2 text-sm font-semibold text-cyan-700 transition hover:border-cyan-300 hover:bg-cyan-50"
              onClick={handleEndGame}
            >
              End game
            </button>
            <button
              className="absolute left-6 top-16 rounded-lg border border-cyan-200 px-4 py-2 text-sm font-semibold text-cyan-700 transition hover:border-cyan-300 hover:bg-cyan-50"
              onClick={() => setShowHelp(true)}
            >
              Help (H)
            </button>
            <div
              className={`absolute right-6 top-6 text-lg font-semibold ${
                timeLeft === 0 ? "text-red-500" : "text-cyan-700"
              }`}
            >
              {String(minutes).padStart(2, "0")}:
              {String(seconds).padStart(2, "0")}
            </div>
            <div className="flex flex-1 flex-col items-center justify-center space-y-3">
              <div className="text-sm text-gray-500">
                Question {currentIndex + 1} of {totalQuestions}
              </div>
              <div className="text-lg font-semibold uppercase tracking-wide text-teal-500">
                {category || "General"}
              </div>
              <div className="text-4xl font-semibold text-gray-800">
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
                    disabled={
                      isSubmitting || answerResult !== null || timeLeft === 0
                    }
                  >
                    {label}
                    {options[index] ? `: ${options[index]}` : ""}
                  </button>
                ))}
              </div>
              {showCorrectAnswerText ? (
                <div className="text-sm font-semibold text-emerald-600">
                  Correct answer: {correctAnswerDisplay}
                </div>
              ) : null}
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
        {showHelp ? (
          <div className="absolute inset-0 z-10 flex items-center justify-center bg-black/40 p-6">
            <div className="w-full max-w-lg rounded-2xl bg-white p-6 text-left shadow-xl">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <h2 className="text-xl font-semibold text-cyan-700">
                    How to play
                  </h2>
                  <p className="mt-2 text-sm text-gray-600">
                    Answer each question before the timer runs out. Your score
                    increases with correct answers and faster responses.
                  </p>
                </div>
                <button
                  className="rounded-lg border border-gray-200 px-3 py-1 text-xs font-semibold text-gray-600 transition hover:bg-gray-100"
                  onClick={() => setShowHelp(false)}
                >
                  Close
                </button>
              </div>
              <ul className="mt-4 space-y-2 text-sm text-gray-600">
                <li>- Click an answer to lock it in.</li>
                <li>- Correct answers turn green, wrong answers turn red.</li>
                <li>- Use the Next button to move on.</li>
                <li>- Press H any time to open this panel.</li>
              </ul>
            </div>
          </div>
        ) : null}
      </div>
    </div>
  );
}
