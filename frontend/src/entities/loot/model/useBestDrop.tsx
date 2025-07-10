'use client'

import { useQuery } from '@tanstack/react-query'

import { getBestDrop } from '../api/loot'

type UseBestDropProps = {
  userId: string
}

export const useBestDrop = ({ userId }: UseBestDropProps) => {
  return useQuery({
    queryKey: ['best-drop', userId],
    queryFn: () => getBestDrop(userId),
    enabled: Boolean(userId)
  })
}
