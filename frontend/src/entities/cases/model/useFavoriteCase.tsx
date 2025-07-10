'use client'

import { useQuery } from '@tanstack/react-query'

import { getUserFavoriteCase } from '../api/cases'

type UseFavoriteCaseProps = {
  userId: string
}

export const useFavoriteCase = ({ userId }: UseFavoriteCaseProps) => {
  return useQuery({
    queryKey: ['favorite-case', userId],
    queryFn: () => getUserFavoriteCase(userId),
    enabled: Boolean(userId)
  })
}
