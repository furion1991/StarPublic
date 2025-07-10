'use client'

import { UserStatisticsContext } from '@/app/providers'
import { useContext } from 'react'

export const useUserStatistics = () => {
  return useContext(UserStatisticsContext)
}
