import React from "react"
import { DotNetifyProvider } from "use-dotnetify"
import HelloWorld from "./HelloWorld"

function App() {
  return (
    <DotNetifyProvider>
      <HelloWorld />
    </DotNetifyProvider>
  )
}

export default App
