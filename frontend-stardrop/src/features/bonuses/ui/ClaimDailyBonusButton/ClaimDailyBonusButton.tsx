'use client'

import { useMutation, useQueryClient } from '@tanstack/react-query'

import { Button } from '@/shared/ui'

import { claimDailyBonus } from '../../api/bonuses'

export const ClaimDailyBonusButton = () => {
  const queryClient = useQueryClient()

  const { mutate, isPending } = useMutation({
    mutationFn: claimDailyBonus,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['me']
      })
    }
  })

  return (
    <Button
      loading={isPending}
      onClick={() => {
        mutate()
      }}
    >
      Забрать
    </Button>
  )
}
