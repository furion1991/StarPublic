'use client'

import { useMutation, useQueryClient } from '@tanstack/react-query'

import { openFewCases } from '../api/cases'

import type { Loot } from '@/entities/loot'

type DroppedLoot = Loot & {
  inventoryId: string
}

type UseOpenFewCasesProps = {
  onSuccess: ({ droppedLootItems }: { droppedLootItems: DroppedLoot[] }) => void
}

export const useOpenFewCases = ({ onSuccess }: UseOpenFewCasesProps) => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: openFewCases,
    onSuccess: ({ data }) => {
      // for balance update
      queryClient.invalidateQueries({
        queryKey: ['me']
      })
      const droppedLootItems = data.result.items.map(
        ({ inventoryRecordId: inventoryId, item }) => ({
          inventoryId,
          ...item
        })
      )

      onSuccess({ droppedLootItems })
    }
  })
}
