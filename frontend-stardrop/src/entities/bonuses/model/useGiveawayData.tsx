'use client'

import { useEffect, useState } from 'react'
import { HubConnectionState } from '@microsoft/signalr'

import { useSignalrConnection } from '@/shared/hooks'

type GiveawayData = {
  currentPrizeAmount: number
  lastWinnerImageUrl: string
  lastWinnerUserId: string
  lastWonPrizeAmount: number
  nextPrizeAmount: number
  secondsRemainingTillDraw: number
  subscribedUsers: number
}

export const useGiveawayData = () => {
  const { connection } = useSignalrConnection('draw')

  const [giveawayData, setGiveawayData] = useState<GiveawayData>()

  useEffect(() => {
    if (!connection) return

    connection.on('prize_draw', setGiveawayData)

    if (connection.state === HubConnectionState.Disconnected) {
      connection.start()
    }

    return () => {
      connection.off('prize_draw', setGiveawayData)
      connection.stop()
    }
  }, [connection])

  return {
    giveawayData
  }
}
