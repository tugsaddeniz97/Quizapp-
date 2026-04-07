import React from "react";

export default function Leaderboard({ players = [], onRefresh }) {
  const sortedPlayers = [...players]
    .filter((player) => player)
    .sort((a, b) => (b.totalScore || 0) - (a.totalScore || 0));

  return (
    <div className="rounded-2xl border border-cyan-200 bg-white p-6 shadow-[0_0_30px_rgba(34,211,238,0.15)]">
      <div className="flex items-center justify-between">
        <h2 className="text-xl font-semibold text-cyan-700">Leaderboard</h2>
        {onRefresh ? (
          <button
            className="rounded-lg border border-cyan-200 px-3 py-1 text-xs font-semibold text-cyan-700 transition hover:border-cyan-300 hover:bg-cyan-50"
            onClick={onRefresh}
          >
            Refresh
          </button>
        ) : null}
      </div>
      <div className="mt-4 space-y-3">
        {sortedPlayers.length === 0 ? (
          <div className="text-sm text-gray-500">No players yet.</div>
        ) : (
          sortedPlayers.map((player, index) => (
            <div
              key={player.playerId || `${player.username}-${index}`}
              className="flex items-center justify-between rounded-lg border border-gray-100 px-3 py-2"
            >
              <div className="flex items-center gap-3">
                <div className="flex h-8 w-8 items-center justify-center rounded-full bg-cyan-100 text-sm font-semibold text-cyan-700">
                  {index + 1}
                </div>
                <div>
                  <div className="text-sm font-semibold text-gray-800">
                    {player.username || "Unknown"}
                  </div>
                  <div className="text-xs text-gray-500">
                    Player #{player.playerId ?? "-"}
                  </div>
                </div>
              </div>
              <div className="text-sm font-semibold text-cyan-700">
                {player.totalScore ?? 0} pts
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
