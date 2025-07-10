'use client'

import { useSignalr } from '@/shared/hooks'
import { LastOpenedCase } from '@/widgets/cases/types/cases.types'
import { createContext, useEffect, useState } from 'react'

import type { HubConnection } from '@microsoft/signalr'

export enum SignalrKeys {
  USERS_ONLINE = 'users_logged_in',
  CASES_OPENED_NUMBER = 'cases_opened_count',
  LAST_OPENED_CASES = 'cases_logs',
  LAST_OPENED_CASES_TOP = 'cases_logs_by_cost'
}

type SignalrData = {
  usersOnlineNumber: number
  casesOpenedNumber: number
  latestOpenedCases: LastOpenedCase[]
  latestOpenedCasesTop: LastOpenedCase[]
}

type SignalrDataContextProps = {
  data: SignalrData
}

export const SignalrDataContext = createContext({} as SignalrDataContextProps)

export const SignalrDataProvider = ({ children }: { children: React.ReactNode }) => {
  const { connection } = useSignalr()

  const [data, setData] = useState<SignalrData>({
    latestOpenedCases: [],
    latestOpenedCasesTop: [],
    casesOpenedNumber: 0,
    usersOnlineNumber: 0
  })

  const subscribeToEvents = (connection: HubConnection) => {
    const connectionsKeys = [
      { connectionKey: SignalrKeys.USERS_ONLINE, objKey: 'usersOnlineNumber' },
      { connectionKey: SignalrKeys.CASES_OPENED_NUMBER, objKey: 'casesOpenedNumber' },
      { connectionKey: SignalrKeys.LAST_OPENED_CASES, objKey: 'latestOpenedCases' },
      { connectionKey: SignalrKeys.LAST_OPENED_CASES_TOP, objKey: 'latestOpenedCasesTop' }
    ]

    connectionsKeys.forEach(({ connectionKey, objKey }) => {
      connection.on(connectionKey, (data: number) => {
        setData((prevData) => ({
          ...prevData,
          [objKey]: data
        }))
      })
    })
  }

  useEffect(() => {
    if (!connection) return

    subscribeToEvents(connection)
  }, [connection])

  return <SignalrDataContext.Provider value={{ data }}>{children}</SignalrDataContext.Provider>
}
