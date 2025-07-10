'use client'

import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { createContext, useEffect, useState } from 'react'

type SignalrContextProps = {
  connection: HubConnection | null
}

export const SignalrContext = createContext({} as SignalrContextProps)

export const SignalrProvider = ({ children }: { children: React.ReactNode }) => {
  const [connection, setConnection] = useState<HubConnection | null>(null)

  const createHubConnection = () => {
    const connect = new HubConnectionBuilder()
      .withUrl(`${process.env.NEXT_PUBLIC_API_BASE}/statistics`, {
        withCredentials: false
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build()

    return connect
  }

  useEffect(() => {
    const connection = createHubConnection()

    connection
      .start()
      .then(() => {
        console.log('Signalr connection success')
      })
      .catch((err) => console.error('Error while connecting to Signalr Hub:', err))

    setConnection(connection)
  }, [])

  return (
    <SignalrContext.Provider
      value={{
        connection
      }}
    >
      {children}
    </SignalrContext.Provider>
  )
}
