'use client'

import { useQueries } from '@tanstack/react-query'

import { getCaseItem } from '../api/loot'

type UseCaseItemProps = {
  itemIds: string[]
}

export const useCaseItem = ({ itemIds }: UseCaseItemProps) => {
  const results = useQueries({
    queries: itemIds.map((id) => ({
      queryKey: ['case-item', id],
      queryFn: () => {
        if (id) {
          return getCaseItem(id)
        }
      }
    }))
  })

  const isLoading = results.some((result) => result.isLoading)
  const allData = results.filter((result) => result.isSuccess).flatMap((result) => result.data)

  return {
    data: allData,
    isLoading
  }
}
