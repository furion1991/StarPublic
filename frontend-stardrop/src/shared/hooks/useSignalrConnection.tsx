'use client'

import { useEffect, useState } from 'react'
import { HubConnectionBuilder, LogLevel, type HubConnection } from '@microsoft/signalr'

export const useSignalrConnection = (endpoint: string) => {
  const [connection, setConnection] = useState<HubConnection | null>(null)

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${process.env.NEXT_PUBLIC_API_BASE}/${endpoint}`, {
        withCredentials: false
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build()

    setConnection(connection)
  }, [])

  return {
    connection
  }
}
