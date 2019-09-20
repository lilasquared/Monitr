import React from "react"
import { useDotNetify } from "use-dotnetify"

const Stats = () => {
  const initialState = {
    Stats: [],
  }

  const [state] = useDotNetify("MonitrVM", initialState)

  return (
    <div className="bg-dark" style={{ flex: "1 0 auto" }}>
      <div className="container">
        <table className="table table-dark">
          <thead>
            <tr>
              <th>ID</th>
              <th>NAME</th>
              <th>CPU</th>
              <th>MEMORY</th>
              <th>NET I/O</th>
              <th>BLOCK I/O</th>
              <th>PIDs</th>
            </tr>
          </thead>
          {state.Stats.map((record: any) => (
            <tr>
              <td>{record.Id}</td>
              <td>{record.Name}</td>
              <td>{record.CpuPercentage}</td>
              <td>
                {record.MemoryUsage} ({record.MemoryPercentage})
              </td>
              <td>{record.NetIO}</td>
              <td>{record.BlockIO}</td>
              <td>{record.PIDs}</td>
            </tr>
          ))}
        </table>
      </div>
    </div>
  )
}

export default Stats
