import React, { useState } from "react";

export default function LoginPage({ onLogin, isLoading = false }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (event) => {
    event.preventDefault();
    const trimmedUsername = username.trim();

    if (!trimmedUsername) {
      setError("Username is required.");
      return;
    }

    setError("");
    onLogin({ username: trimmedUsername, password });
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-red-100 p-6">
      <div className="w-full max-w-md rounded-2xl border border-cyan-200 bg-white p-8 shadow-[0_0_30px_rgba(34,211,238,0.25)]">
        <h1 className="text-3xl font-semibold text-cyan-700">
          Welcome back
        </h1>
        <p className="mt-2 text-sm text-gray-600">
          Sign in to start your quiz session.
        </p>

        <form className="mt-6 space-y-4" onSubmit={handleSubmit}>
          <div>
            <label className="block text-sm font-semibold text-gray-700">
              Username
            </label>
            <input
              type="text"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              className="mt-2 w-full rounded-lg border border-gray-200 px-4 py-2 text-gray-800 focus:border-cyan-400 focus:outline-none focus:ring-2 focus:ring-cyan-200"
              placeholder="Enter your username"
              autoComplete="username"
            />
          </div>
          <div>
            <label className="block text-sm font-semibold text-gray-700">
              Password
            </label>
            <input
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              className="mt-2 w-full rounded-lg border border-gray-200 px-4 py-2 text-gray-800 focus:border-cyan-400 focus:outline-none focus:ring-2 focus:ring-cyan-200"
              placeholder="Enter your password"
              autoComplete="current-password"
            />
          </div>

          {error ? <div className="text-sm text-red-500">{error}</div> : null}

          <button
            type="submit"
            disabled={isLoading}
            className="w-full rounded-lg bg-cyan-500 px-4 py-2 font-semibold text-white transition hover:bg-cyan-600 disabled:cursor-not-allowed disabled:opacity-70"
          >
            {isLoading ? "Signing in..." : "Sign in"}
          </button>
        </form>
      </div>
    </div>
  );
}
