'use client'

import { useQuery } from '@tanstack/react-query'

import { getCaseItem } from '../api/loot'

type UseCaseItemProps = {
  itemId?: string
}

export const useCaseItem = ({ itemId }: UseCaseItemProps) => {
  return useQuery({
    queryKey: ['case-item', itemId],
    queryFn: () => {
      if (itemId) {
        return getCaseItem(itemId)
      }
    },
    enabled: Boolean(itemId)
  })
}
