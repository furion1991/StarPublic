'use client'

import { createContext, useEffect, useState } from 'react'
import type { HubConnection } from '@microsoft/signalr'

import { useSignalrConnection } from '@/shared/hooks'

import type { LastOpenedCase } from '@/widgets/cases/types/cases.types'

export enum UserStatisticsSignlarKeys {
  USERS_ONLINE = 'users_logged_in',
  CASES_OPENED_NUMBER = 'cases_opened_count',
  LAST_OPENED_CASES = 'cases_logs',
  LAST_OPENED_CASES_TOP = 'cases_logs_by_cost'
}

type UserStatistics = {
  usersOnlineNumber: number
  casesOpenedNumber: number
  latestOpenedCases: LastOpenedCase[]
  latestOpenedCasesTop: LastOpenedCase[]
}

type UserStatisticsContextProps = {
  data: UserStatistics
}

export const UserStatisticsContext = createContext({} as UserStatisticsContextProps)

export const UserStatisticsProvider = ({ children }: { children: React.ReactNode }) => {
  const { connection } = useSignalrConnection('statistics')

  const [data, setData] = useState<UserStatistics>({
    latestOpenedCases: [],
    latestOpenedCasesTop: [],
    casesOpenedNumber: 0,
    usersOnlineNumber: 0
  })

  useEffect(() => {
    if (!connection) return

    subscribeToEvents(connection)

    connection.start()
  }, [connection])

  function subscribeToEvents(connection: HubConnection) {
    const connectionsKeys = [
      { connectionKey: UserStatisticsSignlarKeys.USERS_ONLINE, objKey: 'usersOnlineNumber' },
      { connectionKey: UserStatisticsSignlarKeys.CASES_OPENED_NUMBER, objKey: 'casesOpenedNumber' },
      { connectionKey: UserStatisticsSignlarKeys.LAST_OPENED_CASES, objKey: 'latestOpenedCases' },
      {
        connectionKey: UserStatisticsSignlarKeys.LAST_OPENED_CASES_TOP,
        objKey: 'latestOpenedCasesTop'
      }
    ]

    connectionsKeys.forEach(({ connectionKey, objKey }) => {
      connection.on(connectionKey, (data) => {
        setData((prevData) => ({
          ...prevData,
          [objKey]: data
        }))
      })
    })
  }

  return (
    <UserStatisticsContext.Provider
      value={{
        data
      }}
    >
      {children}
    </UserStatisticsContext.Provider>
  )
}
